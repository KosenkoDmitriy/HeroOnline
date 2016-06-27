using UnityEngine;
using System.Collections;

public class Global
{
    //1. version player setting
    //build android trong Other setting :
    //bundle indendtifer : com.blueskysoft.loleng (key ung dung nay la tao trong store googleplay)
    //bunlde verion :2.4 (phien ban ver) nang gia tri len moi khi co update ban moi
    //bundle version code : 17 cung nang len (chi mang y nghia la update thu may)
    //publish setting :
    //1-browse keystore cua googleplay : projec\0-Documents\SignedKeys\lol_unity.keystore    
    //2.copy key nay vao keystore password va alias password cho android trong public setting: psword!@#456
    //3.Key alias : click vao chon thu muc android sdk (va cai jdk java truoc)
    //chinh chat luong do hoa : project setting ->quality -> click vao cac mui ten xuong nam ngang dong default de chon chat luong 
    //do hoa mac dinh la fast , hay good... cho cac platform

    public enum Language
    {
        ENGLISH,
        VIETNAM
    };

    public static bool LOCAL = false;
    public static int version = 12;//khi muon user update ver moi , nang gia tri ver nay len va trong ban database cung nang giong;
    //va build thi nang bunble code trong settting build

    public static Language language = Language.ENGLISH;
}
    