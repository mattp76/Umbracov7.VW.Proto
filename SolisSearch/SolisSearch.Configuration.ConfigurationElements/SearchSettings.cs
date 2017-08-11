using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace SolisSearch.Configuration.ConfigurationElements
{
    public class SearchSettings : ConfigurationElement
    {
        [ConfigurationProperty("defaultField", DefaultValue = "text")]
        public string DefaultField
        {
            get
            {
                return (string)this["defaultField"];
            }
            set
            {
                this["defaultField"] = (object)value;
            }
        }

        [ConfigurationProperty("highlight", DefaultValue = "true")]
        public bool Highlight
        {
            get
            {
                return (bool)this["highlight"];
            }
            set
            {
                this["highlight"] = (object)value;
            }
        }

        [ConfigurationProperty("highlightFields", DefaultValue = "content")]
        public string HighlightFields
        {
            get
            {
                return (string)this["highlightFields"];
            }
            set
            {
                this["highlightFields"] = (object)value;
            }
        }

        [ConfigurationProperty("fragsize", DefaultValue = "200", IsRequired = false)]
        public int Fragsize
        {
            get
            {
                return (int)this["fragsize"];
            }
            set
            {
                this["fragsize"] = (object)value;
            }
        }

        [ConfigurationProperty("fragmenter", DefaultValue = "gap", IsRequired = false)]
        public string Fragmenter
        {
            get
            {
                return (string)this["fragmenter"];
            }
            set
            {
                this["fragmenter"] = (object)value;
            }
        }

        [ConfigurationProperty("indexRoot", DefaultValue = "-1")]
        public string IndexRoot
        {
            get
            {
                return (string)this["indexRoot"];
            }
            set
            {
                this["indexRoot"] = (object)value;
            }
        }

        [ConfigurationProperty("richTextFileTypes", DefaultValue = "txt,pdf,doc,docx,xls,xlsx,ppt,pptx")]
        public string RichTextFileTypes
        {
            get
            {
                return (string)this["richTextFileTypes"];
            }
            set
            {
                this["richTextFileTypes"] = (object)value;
            }
        }

        [ConfigurationProperty("enableLanguageSupport", DefaultValue = "false", IsRequired = false)]
        public bool EnableLanguageSupport
        {
            get
            {
                return (bool)this["enableLanguageSupport"];
            }
            set
            {
                this["enableLanguageSupport"] = (object)value;
            }
        }

        [ConfigurationProperty("enabledLanguages", DefaultValue = "", IsRequired = false)]
        public string EnabledLanguages
        {
            get
            {
                return (string)this["enabledLanguages"];
            }
            set
            {
                this["enabledLanguages"] = (object)value;
            }
        }

        [ConfigurationProperty("defaultOperator", DefaultValue = "", IsRequired = false)]
        public string DefaultOperator
        {
            get
            {
                return (string)this["defaultOperator"];
            }
            set
            {
                this["defaultOperator"] = (object)value;
            }
        }

        [ConfigurationProperty("spellcheck", DefaultValue = "false")]
        public bool SpellCheck
        {
            get
            {
                return (bool)this["spellcheck"];
            }
            set
            {
                this["spellcheck"] = (object)value;
            }
        }
    }
}
