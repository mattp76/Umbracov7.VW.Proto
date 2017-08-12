using AutoMapper;
using SolisSearch.Entities;


namespace SolisSearch.Mapping
{
    public static class AutoMapperWebConfiguration
    {

        public static void Configure()
        {
            ConfigureItemMapping();
        }

        public static void ConfigureItemMapping()
        {
            Mapper.CreateMap<CmsSearchResultItem, CmsSearchResultItemClone>();

        }
    }
 }
