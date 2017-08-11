using SolisSearch.Extensions;
using SolisSearch.Interfaces;
using SolisSearch.Umb.Log;
using System;
using System.Globalization;
using Umbraco.Core.Models;
using Umbraco.Core.Models.EntityBase;


namespace SolisSearch.Umb.CmsEntities
{
    internal class CmsEntityFactory : ICmsEntityFactory
    {
        private readonly LogFacade log = new LogFacade(typeof(CmsEntityFactory));

        public ICmsContent CreateCmsContent(object cmsNode)
        {
            IContent icontent = cmsNode as IContent;
            if (icontent == null)
                return (ICmsContent)null;
            DateTime dateTime1 = ((IEntity)icontent).CreateDate.AsLocalKind();
            DateTime dateTime2 = ((IEntity)icontent).UpdateDate.AsLocalKind();
            DateTime dateTime3 = !icontent.ReleaseDate.HasValue || icontent.ReleaseDate.Value == DateTime.MinValue ? dateTime1 : icontent.ReleaseDate.Value.AsLocalKind();
            DateTime dateTime4 = !icontent.ExpireDate.HasValue || icontent.ExpireDate.Value == DateTime.MinValue ? DateTime.MaxValue.AsUtcKind() : icontent.ExpireDate.Value.AsLocalKind();
            this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, string.Format("Converting cms node with id {0} to UmbracoNode. CreateDate: {1}, UpdateDate: {2}, ReleaseDate: {3}, ExpireDate: {4}", (object)((IEntity)icontent).Id, (object)dateTime1, (object)dateTime2, (object)dateTime3, (object)dateTime4), (Exception)null);
            UmbracoNode umbracoNode = new UmbracoNode()
            {
                ContentType = ((IContentTypeBase)icontent.ContentType).Alias,
                Id = ((IEntity)icontent).Id.ToString((IFormatProvider)CultureInfo.InvariantCulture),
                Name = ((IUmbracoEntity)icontent).Name,
                CreateDate = dateTime1.ToUniversalTime(),
                UpdateDate = dateTime2.ToUniversalTime(),
                StartPublish = dateTime3.ToUniversalTime(),
                EndPublish = dateTime4.ToUniversalTime(),
                NativeNodeObject = (object)icontent,
                Path = ((IUmbracoEntity)icontent).Path,
                ParentId = ((IUmbracoEntity)icontent).ParentId
            };
            this.log.AddLogentry(SolisSearch.Log.Enum.LogLevel.Debug, string.Format("Created UmbracoNode with dates converted to UTC CreateDate: {0}, UpdateDate: {1}, StartPublish: {2}, EndPublish: {3}", (object)umbracoNode.CreateDate, (object)umbracoNode.UpdateDate, (object)umbracoNode.StartPublish, (object)umbracoNode.EndPublish), (Exception)null);
            return (ICmsContent)umbracoNode;
        }

        public ICmsMedia CreateCmsMedia(object cmsMedia)
        {
            IMedia imedia = cmsMedia as IMedia;
            if (imedia == null)
                return (ICmsMedia)null;
            return (ICmsMedia)new UmbracoMedia()
            {
                Id = ((IEntity)imedia).Id.ToString((IFormatProvider)CultureInfo.InvariantCulture),
                Name = ((IUmbracoEntity)imedia).Name,
                NativeMediaObject = (object)imedia
            };
        }

        public ICmsProperty CreateCmsProperty(object property)
        {
            Property property1 = property as Property;
            if (property1 == null)
                return (ICmsProperty)null;
            return (ICmsProperty)new UmbracoProperty()
            {
                Name = property1.Alias,
                Value = property1.Value,
                NativePropertyObject = (object)property1
            };
        }

        public string ActualId(string customId)
        {
            return customId;
        }
    }
}
