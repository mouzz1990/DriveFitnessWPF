using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Word = Microsoft.Office.Interop.Word;
using ZXing.QrCode;
using ZXing;
using ZXing.Common;
using System.IO;
using System.Drawing;

namespace DriveFitnessLibrary
{
    public class ClientCardCreator
    {
        string filenameQrCode;

        string patternCardPath = string.Format(@"{0}\ClientCard.dotm", Environment.CurrentDirectory);

        public void MakeClientCard(Client client)
        {
            CreateQrCodeImage(client);

            CreateWordDocument(client);
        }

        void CreateQrCodeImage(Client client)
        {
            if (!Directory.Exists("ClientCards"))
            {
                Directory.CreateDirectory("ClientCards");
            }

            filenameQrCode = string.Format(@"ClientCards\{0}.jpeg", client);

            try
            {
                if (File.Exists(filenameQrCode))
                    File.Delete(filenameQrCode);

                QRCodeWriter qrEncode = new QRCodeWriter();

                string encString = string.Format("{0}:{1}", client.ClientId, client);

                Dictionary<EncodeHintType, object> hints = new Dictionary<EncodeHintType, object>();    //для колекции поведений
                hints.Add(EncodeHintType.CHARACTER_SET, "utf-8");

                BitMatrix qrMatrix = qrEncode.encode(
                    encString,
                    BarcodeFormat.QR_CODE,
                    300,
                    300,
                    hints
                    );

                BarcodeWriter qrWriter = new BarcodeWriter();

                Bitmap qrImage = qrWriter.Write(qrMatrix);

                qrImage.Save(filenameQrCode, System.Drawing.Imaging.ImageFormat.Jpeg);
                qrImage.Dispose();
            }
            catch
            {
                throw new Exception("Невозможно создать карту клиента. Пожалуйста попробуйте позже или перезапустите приложение");
            }
        }

        void CreateWordDocument(Client client)
        {
            string fileQrPath = string.Format(@"{0}\{1}", Environment.CurrentDirectory, filenameQrCode);

            Word.Application wApp = new Word.Application();
            Word.Document wDoc = wApp.Documents.Add(patternCardPath);

            foreach (Word.Bookmark b in wDoc.Bookmarks)
            {
                if (b.Name == "clientInfo")
                {
                    Word.Range range = b.Range;
                    range.Text = client.ToString();
                }

                if (b.Name == "qrCode")
                {
                    Word.Range rangeB = b.Range;
                    object f = false;
                    object t = true;
                    object left = Type.Missing;
                    object top = Type.Missing;
                    object width = 93;
                    object height = 93;
                    object range = rangeB;
                    Word.WdWrapType wrap = Word.WdWrapType.wdWrapSquare;

                    wDoc.Shapes.AddPicture(fileQrPath, ref f, ref t, ref left, ref top, ref width, ref height, ref range).WrapFormat.Type = wrap;
                }
            }

            wDoc.SaveAs(string.Format(@"{0}\ClientCards\{1}",
                Environment.CurrentDirectory,
                client + ".docx"
                ));

            wApp.Visible = true;
            wApp.Activate();
        }
    }
}
