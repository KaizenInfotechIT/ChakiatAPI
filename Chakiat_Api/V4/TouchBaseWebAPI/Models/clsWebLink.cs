using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TouchBaseWebAPI.Models
{
    /// <summary>
    /// Created By : Nandu
    /// Created On : 25-04-2017
    /// </summary>
    public class clsWebLink
    {
    }

    #region Add record Model classes

    public class clsAddWebLinkInput
    {
        public string WeblinkId { get; set; }

        public string GroupId { get; set; }
        public string CreateBy { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string LinkUrl { get; set; }
    }

    #endregion

    #region Get List Model classes

    public class clsGetWebLinkInput
    {
        public string GroupId { get; set; }
        public string SearchText { get; set; }
    }

    public class clsGetWebLinkOutput
    {
        public string WeblinkId { get; set; }
        public string GroupId { get; set; }

        public string Title { get; set; }
        public string fullDesc { get; set; }
        public string LinkUrl { get; set; }
    }

    #endregion

}