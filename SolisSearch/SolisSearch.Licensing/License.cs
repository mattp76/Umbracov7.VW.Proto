using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolisSearch.Licensing
{
    public class License
    {
        private static string UnlicensedHtml = "<script> document.title =  document.title + ' - Unregistered SolisSearch - Not for production use'; document.body.innerHTML +='<div style=\"{0}\"><a style=\"{2}\" href=\"#\" onclick=\"this.parentNode.style.display=\\'none\\';return false;\">x</a>Solis Search - Unregistered<a style=\"{1}\" target=\"_blank\" href=\"https://www.solissearch.com/my-licenses/\">Get your license</a></div>';</script>";
        private static string DivStyle = "padding:2px;position:fixed;right:40px;border:solid 1px #fff;line-height:12px;border-bottom:none; bottom:0;font-size:12px;width:200px;text-align:center;height:30px;z-index:10000;background-color:#062640; background-position:left center;background-repeat: no-repeat;color:#fff;background-image: url(data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAAsTAAALEwEAmpwYAAAAB3RJTUUH3wINCRQbDAyq0QAAAB1pVFh0Q29tbWVudAAAAAAAQ3JlYXRlZCB3aXRoIEdJTVBkLmUHAAACcElEQVQ4y42US4iOYRTHf+83nxHjMmZkCKGwQJKFBRY0I5dYYKWQWcnKhlIuC5SFJEtNYzFFFhZyTS6NyL0ZUyOxkEuJGPMxN+b7pp/FPC+v1ztx6q1zznvO/znPec7/RPxD1HJgGzAMaIiiqJ//EXUofZza66BU/ys+FzuiKEKdAcR68rw+oAiYjA/6lIx4UFepnerl5MnqWPW9WlArU5U1Bf/a2JdPYE4M9hq1CagHKoBJwCGgDJgagArACWAL8B2Ymdk/dbvar7apm9Rr/i131Hq1Ndi7k1hRRk8WAXOAwweON04eXp6nWCyxY/N6asZXxXmdwEHgaRRFzWmMdJUj1HNHT57xcdtzVe+3tNtw9qLFYilZ6V11VPqV82oFMH3QL8BXYGOuLMfc2TMAWDBnFi9fvaM0MEA+XxbnLgFq1CqgXI2ATzlgA9AOPAvfaoDqyjGcPn+dz18KXLp1j66eXt6+/zh4i98XWwg8AVoCxt4c8Bp4AbQBrcAbgE3r6pg2pYartx9SKg1w494T6jbv5Nb9FhKd6gW6wgSUAYUsxowL8/hLurp7XFO/y2Gzl1k5f6XND+IHdkI6P5dBn+9AQzJoVMVImo7tY/HCefzoL9L8oBWgEej4iyGpFx4fpn+/ejM9gJ86Cp69eMOvXd0Pw8zWhbwoC3RCYlgfqdPUPYFasfSpR9Ra9VVYGlvToxMDngpJneqy1L8Vam3CHq1eCPE/1NVZFc5Vr6jLU22oCokdyeUQCHBBbVeXDrkLM3ZdlVpUP6iVqcMq1JlDXXko8OpQ4ZcYMGvBxvav9RUT+w+CD0oBqA1L9ls6Pq3/BOW0WjD5ZV9cAAAAAElFTkSuQmCC);";
        private static string AnchorStyle = "color:#fff;display:block;font-size:9px;height:9px;text-decoration:underline;";
        private static string CloseStyle = "position:absolute;right:2px;top:-2px;color:#fff;font-size:9px;text-decoration:none;";

        public static bool IsValidated { get; private set; }

        public static string UnlicensedJavascript
        {
            get
            {
                return string.Format(License.UnlicensedHtml, (object)License.DivStyle, (object)License.AnchorStyle, (object)License.CloseStyle);
            }
        }

        public static bool IsValid(string username, string licensekey)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(licensekey))
                return false;
            License.IsValidated = licensekey == License.ComplexLicenseAlgorithm(username);
            return License.IsValidated;
        }

        private static string ComplexLicenseAlgorithm(string username)
        {
            string empty = string.Empty;
            char[] charArray = "ABCDEFGHIJKLMNOPQRSTUVXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVXYZ0123456789".ToCharArray();
            if (username.Length < 16)
                username += "ABCDEFGHIJKLMNOPQRSTUVXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVXYZ0123456789ABCDEFGHIJKLMNOPQRSTUVXYZ0123456789".Substring(username.Length, 16 - username.Length);
            int length = username.Length;
            foreach (byte num in ((IEnumerable<byte>)Encoding.UTF8.GetBytes(username)).ToList<byte>())
            {
                int index = Convert.ToInt32(num);
                while (index > charArray.Length - 1)
                    index -= length;
                if (index < 0)
                    index = 0;
                empty += (string)(object)charArray[index];
                if (empty.Length > 15)
                    break;
            }
            return empty;
        }
    }
}
