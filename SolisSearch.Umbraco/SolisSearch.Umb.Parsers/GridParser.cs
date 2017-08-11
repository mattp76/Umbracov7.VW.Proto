using SolisSearch.Configuration.ConfigurationElements;
using SolisSearch.Interfaces;
using SolisSearch.Umb.Log;
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace SolisSearch.Umb.Parsers
{
    public class GridParser : IPropertyParser
    {
        private readonly LogFacade log = new LogFacade(typeof(GridParser));

        public ICmsContent CurrentCmsNode { get; set; }

        public ICmsProperty CurrentCmsProperty { get; set; }

        public Property CurrentSolisProperty { get; set; }

        public string GetPropertyValue(object cmsPropertyValue)
        {
            if (cmsPropertyValue == null)
                return string.Empty;
            StringBuilder stringBuilder = new StringBuilder(string.Empty);
            try
            {
                foreach (Section section in ((Grid)new DataContractJsonSerializer(typeof(Grid)).ReadObject((Stream)new MemoryStream(Encoding.UTF8.GetBytes(cmsPropertyValue.ToString())))).sections)
                {
                    foreach (Row row in section.rows)
                    {
                        foreach (Area area in row.areas)
                        {
                            try
                            {
                                foreach (Control control in area.controls)
                                {
                                    if (!(control.value.GetType() == typeof(object)))
                                        stringBuilder.AppendLine(control.value.ToString());
                                }
                            }
                            catch (Exception ex)
                            {
                                this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Error, "Error iterating controls", ex);
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, "Error deserializing grid json " + cmsPropertyValue, ex);
                throw;
            }
            return stringBuilder.ToString();
        }
    }
}
