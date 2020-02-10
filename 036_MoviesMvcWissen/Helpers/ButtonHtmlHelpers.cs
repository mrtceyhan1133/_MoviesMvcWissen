using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _036_MoviesMvcWissen.Helpers
{
    public static class ButtonHtmlHelpers
    {
        /// <summary>
        /// Tipi Submit olan bir Save button oluşturur.
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="name">buttonun üstünde yazan isim</param>
        /// <returns></returns>
        public static MvcHtmlString SaveButton(this HtmlHelper htmlHelper,string name)
        {
            TagBuilder tagBuilder = new TagBuilder("button");
            tagBuilder.InnerHtml = name;
            tagBuilder.MergeAttribute("class", "btn btn-success");
            tagBuilder.MergeAttribute("type", "submit");
            return MvcHtmlString.Create(tagBuilder.ToString());
        }
        /// <summary>
        /// Parametrelere göre custom button oluşturur.
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="buttonText">Buton Yazısı</param>
        /// <param name="buttonClass">buton classı</param>
        /// <param name="buttonType"></param>
        /// <returns></returns>
        public static MvcHtmlString Button(this HtmlHelper htmlHelper,string buttonText,string buttonClass, string buttonType="")
        {
            TagBuilder tagBuilder = new TagBuilder("button");
            tagBuilder.InnerHtml = buttonText;
            if (buttonType != "")
                tagBuilder.MergeAttribute("type", buttonType);
            tagBuilder.MergeAttribute("class", buttonClass);
            return MvcHtmlString.Create(tagBuilder.ToString());
        }
    }
}