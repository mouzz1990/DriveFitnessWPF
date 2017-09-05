using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace DriveFitnessLibrary
{
    public class ReportManager
    {
        DrivefitnessContext DriveContext;
        Messager messager;

        public ReportManager()
        {
            DriveContext = new DrivefitnessContext();
            messager = new Messager();
        }

        public List<Group> GetGroups()
        {
           List<Group> groups = DriveContext.Group.AsNoTracking().ToList();
            return groups;
        }

        public DataTable GetAttendanceTable(Group gr, DateTime startDate, DateTime endDate)
        {
            DataTable attendanceTable = new DataTable("Attendance");
            attendanceTable.Columns.Add("Клиент");

            //Получение дат посещения и дат покупки абонементов в дни не посещения занятий
            var DatesVisit = (from at in DriveContext.Attendance
                              join client in DriveContext.Client on at.ClientId equals client.ClientId
                              where at.DateVisit >= startDate && at.DateVisit <= endDate && at.Client.GroupId == gr.GroupId
                              select at.DateVisit).Distinct().ToList();

            HashSet<DateTime> visitHash = new HashSet<DateTime>(DatesVisit);

            var DatesSubBuy = (from sub in DriveContext.Subscription
                               join client in DriveContext.Client on sub.ClientSubscriptionId equals client.ClientId
                               where client.GroupId == gr.GroupId && sub.SubscriptionDate >= startDate && sub.SubscriptionDate <= endDate
                               select sub.SubscriptionDate).Distinct().ToList();

            HashSet<DateTime> subHash = new HashSet<DateTime>(DatesSubBuy);

            subHash.ExceptWith(visitHash);
            visitHash.UnionWith(subHash);

            List<DateTime> Dates = visitHash.ToList();
            Dates.Sort();

            foreach (var d in Dates)
                attendanceTable.Columns.Add(d.ToString("dd-MM-yy"));

            attendanceTable.Columns.Add("Оплата");

            var subscriptions = (from sub in DriveContext.Subscription
                               join client in DriveContext.Client on sub.ClientSubscriptionId equals client.ClientId
                               where client.GroupId == gr.GroupId && sub.SubscriptionDate >= startDate && sub.SubscriptionDate <= endDate
                               select sub).ToList();

            var attendances = from att in DriveContext.Attendance.AsNoTracking()
                              join client in DriveContext.Client on att.ClientId equals client.ClientId
                              where client.GroupId == gr.GroupId && att.DateVisit >= startDate && att.DateVisit <= endDate
                              select att;

            var clients = from c in DriveContext.Client
                          where c.GroupId == gr.GroupId
                          select c;

            var groupsByClient = (from a in attendances
                       group a by a.ClientId).ToList();

            foreach (IGrouping<int,Attendance> groupEl in groupsByClient)
            {
                DataRow row = attendanceTable.NewRow();

                Client client = clients.Single(x => x.ClientId == groupEl.Key);

                float price = 0;

                row[0] = client;

                //Получение суммы средств за абонементы
                var subs = subscriptions.Where(x => x.ClientSubscriptionId == groupEl.Key && 
                                                    x.SubscriptionDate >= startDate && 
                                                    x.SubscriptionDate <= endDate).ToList();
                foreach (var s in subs)
                    price += s.SubscriptionPrice;


                foreach (var groupItem in groupEl)
                {
                    //посещение за кэш
                    foreach (var d in Dates)
                    {
                        if (d == groupItem.DateVisit)
                        {
                            row[d.ToString("dd-MM-yy")] = groupItem.Payment;
                            price += groupItem.AttendancePrice ?? 0;
                        }
                    }
                    //учет абонементов
                    foreach (var s in subscriptions)
                    {
                        if (s.ClientSubscriptionId == groupItem.ClientId)
                        {
                            //покупка и посещение в один день
                            if (s.SubscriptionDate >= startDate && s.SubscriptionDate <= endDate)
                            {
                                row[s.SubscriptionDate.ToString("dd-MM-yy")] = "A → " + s.SubscriptionPrice;
                            }
                            //покупка но посещение в другой день
                            foreach (var sh in subHash)
                            {
                                if (s.SubscriptionDate == sh)
                                {
                                    row[s.SubscriptionDate.ToString("dd-MM-yy")] = "A : " + s.SubscriptionPrice;
                                }
                            }
                        }
                    }
                }

                row["Оплата"] = price;

                attendanceTable.Rows.Add(row);
            }

            //ИТОГО:
            float sum = 0;
            foreach (DataRow r in attendanceTable.Rows)
            {
                sum += float.Parse(r["Оплата"].ToString());
            }

            DataRow rowSum = attendanceTable.NewRow();
            rowSum["Оплата"] = sum;
            rowSum["Клиент"] = "Итого:";
            attendanceTable.Rows.Add(rowSum);


            return attendanceTable;
        }

        public void CreateReport(DataTable data, string saveToPath)
        {
            if (data == null)
            {
                messager.ErrorMessage("Ошибка получения отчетной таблицы! Повторите попытку или обратитесь к разработчику.");
                return;
            }

            if (string.IsNullOrEmpty(saveToPath) || string.IsNullOrWhiteSpace(saveToPath))
            {
                messager.ErrorMessage(string.Format("Ошибка в указанном пути файла:{1}\"{0}\",{1}укажите правильный путь и имя файла!",
                    saveToPath,
                    Environment.NewLine
                    ));

                return;
            }

            if (!Directory.Exists(Path.GetDirectoryName(saveToPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(saveToPath));

            try
            {
                Excel.Application eApp = new Excel.Application();
                Excel.Workbook wBook = eApp.Workbooks.Add();
                Excel.Worksheet wSheet = wBook.Worksheets.Add();
                
                for (int i = 0; i < data.Columns.Count; i++)
                {
                    var header = data.Columns[i].ColumnName;
                    header = header.Replace("-", " ");
                    wSheet.Cells[1, i + 1] = header;
                }

                for (int i = 0; i < data.Rows.Count; i++)
                {
                    for (int j = 0; j < data.Columns.Count; j++)
                    {
                        wSheet.Cells[i + 2, j + 1] = data.Rows[i][j];
                    }
                }

                //finding a last cell
                Excel.Range r1 = wSheet.Cells.get_End(Excel.XlDirection.xlToRight);
                Excel.Range r2 = wSheet.Cells.get_End(Excel.XlDirection.xlDown);

                string rightEnd = r1.get_Address().Split('$')[1];   //letter
                string bottomEnd = r2.get_Address().Split('$')[2];  //number

                //last cell address
                string lastCell = rightEnd + bottomEnd;

                //painting grid
                List<Excel.XlBordersIndex> borders = new List<Excel.XlBordersIndex>()
                {
                    Excel.XlBordersIndex.xlEdgeRight,
                    Excel.XlBordersIndex.xlEdgeLeft,
                    Excel.XlBordersIndex.xlEdgeBottom,
                    Excel.XlBordersIndex.xlEdgeTop,
                    Excel.XlBordersIndex.xlInsideHorizontal,
                    Excel.XlBordersIndex.xlInsideVertical
                };

                foreach (Excel.XlBordersIndex brd in borders)
                {
                    Excel.Range r = wSheet.get_Range("A1", lastCell);

                    r.Borders[brd].Weight = Excel.XlBorderWeight.xlThin;
                    r.Borders[brd].LineStyle = Excel.XlLineStyle.xlContinuous;
                    r.Borders[brd].ColorIndex = 0;
                }

                wSheet.get_Range("A1", lastCell).Font.Size = 14;

                wSheet.Columns.AutoFit();

                wBook.SaveAs(saveToPath);
                eApp.Visible = true;
            }
            catch
            {
                messager.ErrorMessage(string.Format("Ошибка создания документа Microsoft Office Excel - \"{0}\"{1}{1}Обратитесь к разработчику!",
                    Path.GetFileName(saveToPath),
                    Environment.NewLine
                    ));
            }
        }
        
    }
}
