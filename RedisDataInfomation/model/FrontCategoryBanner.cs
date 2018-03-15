using RedisDataInfomation.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisDataInfomation.model
{
    /// <summary>
    /// 前台分類Banner
    /// </summary>
    [Serializable]
    public class FrontCategoryBanner
    {
        /// <summary>
        /// Banner代碼
        /// </summary>
        public int BannerID { get; set; }
        /// <summary>
        /// 前台分類代碼
        /// </summary>
        public int CateID { get; set; }
        /// <summary>
        /// Banner圖片連結位置
        /// </summary>
        public string CateBanner { get; set; }
        /// <summary>
        /// Banner類型
        /// </summary>
        public CategoryEnum.CATE_BANNER_TYPE BannerType { get; set; }
        /// <summary>
        /// Banner連結
        /// </summary>
        public string BannerUrl { get; set; }
    }
}
