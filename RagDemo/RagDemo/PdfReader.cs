using System;
using System.Collections.Generic;
using System.Text;
using UglyToad.PdfPig;

namespace RagDemo
{
    public class PdfReader
    {
        public static string Read(string filePath)
        {
            StringBuilder sb = new();

            using var pdf = PdfDocument.Open(filePath);

            foreach (var page in pdf.GetPages())
            {
                sb.AppendLine(page.Text);
            }

            return sb.ToString();
        }
    }
}
