using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ThoughtWorks.QRCode.Codec
{
    public class QRCodeEncoder
    {
        // public QRCodeEncoder();

        public virtual Color QRCodeBackgroundColor { get; set; }
        public virtual ENCODE_MODE QRCodeEncodeMode { get; set; }
        public virtual ERROR_CORRECTION QRCodeErrorCorrect { get; set; }
        public virtual Color QRCodeForegroundColor { get; set; }
        public virtual int QRCodeScale { get; set; }
        public virtual int QRCodeVersion { get; set; }

        // public virtual bool[][] calQrcode(byte[] qrcodeData);
        // public virtual int calStructureappendParity(sbyte[] originaldata);
        // public virtual Bitmap Encode(string content);
        //public virtual Bitmap Encode(string content, Encoding encoding);
        // public virtual void setStructureappend(int m, int n, int p);

        public enum ENCODE_MODE
        {
            ALPHA_NUMERIC = 0,
            NUMERIC = 1,
            BYTE = 2
        }
        public enum ERROR_CORRECTION
        {
            L = 0,
            M = 1,
            Q = 2,
            H = 3
        }
    }
}