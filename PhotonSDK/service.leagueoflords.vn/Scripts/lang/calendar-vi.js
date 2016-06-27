// ** I18N

// Calendar EN language
// Author: Mihai Bazon, <mihai_bazon@yahoo.com>
// Encoding: UTF-8
// Distributed under the same terms as the calendar itself.

// For translators: please use UTF-8 if possible.  We strongly believe that
// Unicode is the answer to a real internationalized world.  Also please
// include your contact information in the header, as can be seen above.

// full day names
Calendar._DN = new Array
("Chủ Nhật",
 "Thứ Hai",
 "Thứ Ba",
 "Thứ Tư",
 "Thứ Năm",
 "Thứ Sáu",
 "Thứ Bảy",
 "Chủ Nhật");

// Please note that the following array of short day names (and the same goes
// for short month names, _SMN) isn't absolutely necessary.  We give it here
// for exemplification on how one can customize the short day names, but if
// they are simply the first N letters of the full name you can simply say:
//
//   Calendar._SDN_len = N; // short day name length
//   Calendar._SMN_len = N; // short month name length
//
// If N = 3 then this is not needed either since we assume a value of 3 if not
// present, to be compatible with translation files that were written before
// this feature.

// short day names
Calendar._SDN = new Array
("Nhật",
 "Hai",
 "Ba",
 "Tư",
 "Năm",
 "Sáu",
 "Bảy",
 "Nhật");

// First day of the week. "0" means display Sunday first, "1" means display
// Monday first, etc.
Calendar._FD = 1;

// full month names
Calendar._MN = new Array
("Tháng 1",
 "Tháng 2",
 "Tháng 3",
 "Tháng 4",
 "Tháng 5",
 "Tháng 6",
 "Tháng 7",
 "Tháng 8",
 "Tháng 9",
 "Tháng 10",
 "Tháng 11",
 "Tháng 12");

// short month names
Calendar._SMN = new Array
("Th1",
 "Th2",
 "Th3",
 "Th4",
 "Th5",
 "Th6",
 "Th7",
 "Th8",
 "Th9",
 "T10",
 "T11",
 "T12");

// tooltips
Calendar._TT = {};
Calendar._TT["INFO"] = "Trợ giúp";

Calendar._TT["ABOUT"] =
"DHTML Date/Time Selector\n" +
"(c) dynarch.com 2002-2005 / Author: Mihai Bazon\n" + // don't translate this this ;-)
"Để có phiên bản mới nhất: http://www.dynarch.com/projects/calendar/\n" +
"Distributed under GNU LGPL.  See http://gnu.org/licenses/lgpl.html for details." +
"\n\n" +
"Chọn ngày:\n" +
"- Sử dụng nút \xab, \xbb để chọn năm\n" +
"- Sử dụng nút " + String.fromCharCode(0x2039) + ", " + String.fromCharCode(0x203a) + " để chọn tháng\n" +
"- Giữ chuột ở các nút trên để chọn nhanh.";
Calendar._TT["ABOUT_TIME"] = "\n\n" +
"Chọn giờ:\n" +
"- Nhấn vào ô giờ/phút để tăng\n" +
"- hoặc nhấn và giữ Shift để giảm\n" +
"- hoặc nhấn và kéo để chọn nhanh.";

Calendar._TT["PREV_YEAR"] = "Năm trước (giữ chuột hiện menu)";
Calendar._TT["PREV_MONTH"] = "Tháng trước (giữ chuột hiện menu)";
Calendar._TT["GO_TODAY"] = "Hôm nay";
Calendar._TT["NEXT_MONTH"] = "Tháng sau (giữ chuột hiện menu)";
Calendar._TT["NEXT_YEAR"] = "Năm sau (giữ chuột hiện menu)";
Calendar._TT["SEL_DATE"] = "Chọn ngày";
Calendar._TT["DRAG_TO_MOVE"] = "Kéo di chuyển";
Calendar._TT["PART_TODAY"] = " (hôm nay)";

// the following is to inform that "%s" is to be the first day of week
// %s will be replaced with the day name.
Calendar._TT["DAY_FIRST"] = "Hiện %s trước";

// This may be locale-dependent.  It specifies the week-end days, as an array
// of comma-separated numbers.  The numbers are from 0 to 6: 0 means Sunday, 1
// means Monday, etc.
Calendar._TT["WEEKEND"] = "0,6";

Calendar._TT["CLOSE"] = "Đóng";
Calendar._TT["TODAY"] = "Hôm nay";
Calendar._TT["TIME_PART"] = "Nhấn chuột hoặc kéo để thay đổi giá trị";

// date formats
Calendar._TT["DEF_DATE_FORMAT"] = "%Y-%m-%d";
Calendar._TT["TT_DATE_FORMAT"] = "%Y-%m-%d";

Calendar._TT["WK"] = "tuần";
Calendar._TT["TIME"] = "Giờ:";
