using RedisDataInfomation.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisDataInfomation.model
{
    /// <summary>
    /// 前台分類資訊
    /// </summary>
    [Serializable]
    public class FrontCategoryInfo
    {
        /// <summary>
        /// 前台分類代碼
        /// </summary>
        public int CateID { get; set; }
        /// <summary>
        /// 前台分類名稱
        /// </summary>
        public string CateName { get; set; }
        /// <summary>
        /// 前台分類商品數量
        /// </summary>
        public int? CateAmount { get; set; }
        /// <summary>
        /// 前台分類型態
        /// </summary>
        public CategoryEnum.CATE_TYPE CateType { get; set; }
        /// <summary>
        /// 大網行銷頁面連結
        /// </summary>
        public string ETMallPromoUrl { get; set; }
        /// <summary>
        /// 小網行銷頁面連結
        /// </summary>
        public string MobiPromoUrl { get; set; }
        /// <summary>
        /// APP行銷頁面連結
        /// </summary>
        public string AppPromoUrl { get; set; }
        /// <summary>
        /// 是否展開分類
        /// </summary>
        public bool Highlight { get; set; }
        /// <summary>
        /// 展開分類種類
        /// </summary>
        public CategoryEnum.HIGHLIGHT_TYPE HighlightType { get; set; }
        /// <summary>
        /// 前台父分類代碼
        /// </summary>
        public int ParentID { get; set; }
        /// <summary>
        /// 前台分類代碼階層組合 [x-x-x-x]
        /// </summary>
        public string CombinedCode { get; set; }
        /// <summary>
        /// 館別代碼
        /// </summary>
        public int StoreID { get; set; }
        /// <summary>
        /// 排序代碼
        /// </summary>
        public int OrderID { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        public char Enabled { get; set; }
        /// <summary>
        /// 分類版塊(ETMall一般館為0)
        /// </summary>
        public int? CategoryPlate { get; set; }
        /// <summary>
        /// 分類年齡分級?(DB文件沒寫)
        /// </summary>
        public string CategoryAgeRating { get; set; }
        /// <summary>
        /// 是否為最後一層分類?(DB文件沒寫)
        /// </summary>
        public bool IsLastLevel { get; set; }
        /// <summary>
        /// 商品數量?(DB文件沒寫)
        /// </summary>
        public int ProductCount { get; set; }
        /// <summary>
        /// 分類Banner
        /// </summary>
        public List<FrontCategoryBanner> CategoryBanner { get; set; }
    }
}
