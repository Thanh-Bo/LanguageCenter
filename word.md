BỘ CÔNG THƯƠNG
TRƯỜNG ĐẠI HỌC CÔNG THƯƠNG TP. HCM
KHOA CÔNG NGHỆ THÔNG TIN
------
 


KHÓA LUẬN CỬ NHÂN


XÂY DỰNG HỆ THỐNG QUẢN LÝ TRUNG TÂM NGOẠI NGỮ

Ngành: Công Nghệ Thông Tin

SINH VIÊN THỰC HIỆN:
	1. 2001230318 – Trần Ngọc Trúc Huỳnh – 14DHTH14
	2. 2001230317 – Lê Thị Thanh Huyền – 14DHTH14
	3. 2001230895 – Nguyễn Văn Thành – 14DHTH14




TP. HỒ CHÍ MINH, tháng 11 năm 2026
BỘ CÔNG THƯƠNG
TRƯỜNG ĐẠI HỌC CÔNG THƯƠNG TP. HCM
KHOA CÔNG NGHỆ THÔNG TIN
------
 


KHÓA LUẬN CỬ NHÂN


XÂY DỰNG HỆ THỐNG QUẢN LÝ TRUNG TÂM NGOẠI NGỮ

Ngành: Công Nghệ Thông Tin 

GIẢNG VIÊN HƯỚNG DẪN: 
ThS. Lâm Thị Họa Mi

SINH VIÊN THỰC HIỆN:
	1. 2001230318 – Trần Ngọc Trúc Huỳnh – 14DHTH14
	2. 2001230317 – Lê Thị Thanh Huyền – 14DHTH14
	3. 2001230895 – Nguyễn Văn Thành – 14DHTH14

TP. HỒ CHÍ MINH, tháng 11 năm 2026 
LỜI CẢM ƠN
Trong suốt quá trình học tập và thực hiện khóa luận cử nhân với đề tài “Xây dựng hệ thống quản lý Trung tâm Ngoại ngữ”, nhóm đã nhận được sự quan tâm, hướng dẫn và hỗ trợ từ quý Thầy/Cô, gia đình và bạn bè. Đây là nguồn động viên quý báu giúp nhóm có thêm động lực để hoàn thành khóa luận.
Trước hết, nhóm xin bày tỏ lòng biết ơn sâu sắc đến Cô Lâm Thị Họa Mi, giảng viên hướng dẫn, đã tận tình hướng dẫn, định hướng và đóng góp những ý kiến quý báu trong suốt quá trình nhóm thực hiện đề tài. Những góp ý và sự hỗ trợ của Cô đã giúp nhóm từng bước hoàn thiện nội dung, cũng như nâng cao tính thực tiễn và chất lượng của hệ thống.
Nhóm xin chân thành cảm ơn Ban Giám hiệu Trường Đại học Công Thương Thành phố Hồ Chí Minh, cùng Khoa Công nghệ Thông tin đã tạo điều kiện thuận lợi trong quá trình học tập, nghiên cứu và thực hiện khóa luận. Những kiến thức và kỹ năng được trang bị trong quá trình học tập tại trường là nền tảng quan trọng để nhóm có thể vận dụng vào việc phân tích, thiết kế và xây dựng hệ thống trong đề tài.
Bên cạnh đó, nhóm xin gửi lời cảm ơn đến gia đình và bạn bè đã luôn động viên, hỗ trợ và tạo điều kiện để nhóm có thể tập trung hoàn thành khóa luận.
Mặc dù nhóm đã cố gắng hoàn thiện đề tài trong phạm vi kiến thức và thời gian cho phép, khóa luận chắc chắn vẫn không tránh khỏi những thiếu sót và hạn chế. Nhóm rất mong nhận được những ý kiến đóng góp và nhận xét từ quý Thầy/Cô để có thể tiếp tục hoàn thiện đề tài cũng như nâng cao kiến thức và kinh nghiệm của bản thân.Một lần nữa, nhóm xin chân thành cảm ơn quý Thầy/Cô, gia đình và bạn bè đã đồng hành và hỗ trợ nhóm trong suốt quá trình thực hiện khóa luận.



NHẬN XÉT CỦA GIẢNG VIÊN HƯỚNG DẪN

	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	GIẢNG VIÊN HƯỚNG DẪN
	(Ký và ghi rõ họ tên)

 MỤC LỤC














 DANH MỤC CÁC KÝ HIỆU VÀ CHỮ VIẾT TẮT

Viết tắt	Tiếng Anh	Tiếng Việt
DBMS	Database Management System	Hệ quản trị cơ sở dữ liệu
IT	Information Technology	Công nghệ thông tin
....























 DANH MỤC HÌNH
Hình 1.1: Sơ đồ hệ thống	7
Hình 1.2: Kiến trúc hệ thống	10
…
(Tạo mục lục tự động cho đề mục hình)






















 DANH MỤC BẢNG
Bảng 1.1: Dữ liệu thực nghiệm	28
Bảng 1.2: Danh sách người dùng	30
…
(Tạo mục lục tự động cho đề mục bảng nếu có)



















 
MỞ ĐẦU
1. GIỚI THIỆU 
Trong bối cảnh nhu cầu học ngoại ngữ ngày càng được quan tâm, việc tổ chức và quản lý các hoạt động tại các trung tâm ngoại ngữ ngày càng trở nên cần thiết. Trung tâm phải quản lý nhiều thông tin liên quan đến quá trình hoạt động và đào tạo, đồng thời có sự tham gia của nhiều nhóm người dùng khác nhau. Khi số lượng thông tin và người sử dụng ngày càng tăng, việc quản lý một cách thống nhất, chính xác và thuận tiện là vấn đề cần được chú trọng.
Trong thực tế, các hoạt động của trung tâm có sự liên quan giữa nhiều bộ phận và đối tượng sử dụng. Mỗi nhóm người dùng có vai trò và nhu cầu khác nhau trong quá trình làm việc và học tập. Vì vậy, việc xây dựng một hệ thống quản lý phù hợp sẽ giúp hỗ trợ các hoạt động của trung tâm, tạo sự thuận tiện trong việc quản lý và sử dụng thông tin, đồng thời góp phần nâng cao hiệu quả hoạt động.
Xuất phát từ những yêu cầu trên, nhóm lựa chọn đề tài “Xây dựng hệ thống quản lý Trung tâm Ngoại ngữ” nhằm nghiên cứu và xây dựng một hệ thống hỗ trợ công tác quản lý tại trung tâm. Đề tài được thực hiện thông qua các giai đoạn khảo sát, phân tích yêu cầu, phân tích và thiết kế hệ thống, xây dựng phần mềm, kiểm thử và hoàn thiện sản phẩm. Trong quá trình thực hiện, nhóm tập trung tìm hiểu các nghiệp vụ của trung tâm và nhu cầu của các nhóm người dùng để làm cơ sở xây dựng hệ thống phù hợp.
Hệ thống được định hướng xây dựng theo mô hình quản lý tập trung, hỗ trợ các nhóm người dùng trong quá trình thực hiện công việc và học tập, đồng thời đảm bảo thông tin được quản lý thống nhất giữa các thành phần của hệ thống. Theo tài liệu, hệ thống được phát triển thành các phân hệ phù hợp với từng nhóm người dùng và sử dụng cơ sở dữ liệu tập trung để quản lý dữ liệu. Qua đó, đề tài hướng đến việc xây dựng một hệ thống có khả năng đáp ứng các nhu cầu quản lý cơ bản của Trung tâm Ngoại ngữ, góp phần nâng cao hiệu quả quản lý, hỗ trợ người sử dụng và tạo nền tảng cho việc phát triển hệ thống trong tương lai.
2. MỤC TIÊU ĐỀ TÀI
Đề tài “Xây dựng hệ thống quản lý Trung tâm Ngoại ngữ” nhằm xây dựng một hệ thống hỗ trợ trung tâm trong việc quản lý các hoạt động và thông tin liên quan đến quá trình đào tạo, đồng thời nâng cao hiệu quả trong công tác quản lý và phục vụ học viên. Đề tài hướng đến việc xây dựng một môi trường quản lý tập trung, giúp các thông tin được lưu trữ và xử lý một cách thống nhất, thuận tiện cho việc cập nhật, tra cứu và theo dõi trong quá trình hoạt động của trung tâm. Bên cạnh đó, hệ thống được xây dựng nhằm hỗ trợ các bộ phận trong trung tâm phối hợp và thực hiện công việc thuận tiện hơn, giảm thiểu những hạn chế của phương pháp quản lý thủ công, hạn chế sai sót và tiết kiệm thời gian trong quá trình xử lý thông tin. Đối với học viên, hệ thống tạo điều kiện thuận lợi trong việc tiếp cận thông tin, thực hiện các nhu cầu liên quan đến quá trình học tập và theo dõi tình trạng học tập của bản thân. Qua đó, đề tài hướng đến việc xây dựng một hệ thống có giao diện dễ sử dụng, đáp ứng được các nhu cầu quản lý cơ bản của Trung tâm Ngoại ngữ, góp phần nâng cao chất lượng phục vụ, hiệu quả quản lý và tạo nền tảng để trung tâm có thể tiếp tục mở rộng, phát triển hệ thống trong tương lai
3. ĐỐI TƯỢNG VÀ PHẠM VI ĐỀ TÀI
Đối tượng nghiên cứu của đề tài là các hoạt động quản lý và đào tạo tại Trung tâm Ngoại ngữ, tập trung vào nhu cầu sử dụng và xử lý thông tin của các nhóm người dùng tham gia vào quá trình hoạt động của trung tâm, gồm quản trị viên, nhân viên học vụ, nhân viên tư vấn, giáo viên, trợ giảng, nhân viên tài chính và học viên. Trong quá trình thực hiện, đề tài tiến hành khảo sát và phân tích các nghiệp vụ của trung tâm, thu thập yêu cầu từ các nhóm người dùng để làm cơ sở xây dựng hệ thống quản lý phù hợp với nhu cầu thực tế. Phạm vi của đề tài được giới hạn trong việc phân tích, thiết kế và xây dựng hệ thống hỗ trợ quản lý các hoạt động của Trung tâm Ngoại ngữ, từ quá trình khảo sát yêu cầu, phân tích hệ thống, thiết kế cơ sở dữ liệu, giao diện và phân quyền người dùng đến xây dựng, kiểm thử và hoàn thiện sản phẩm. Hệ thống được triển khai thành các phân hệ phù hợp với từng nhóm người dùng, trong đó tài liệu xác định ứng dụng WinForms dành cho quản trị viên và Website dành cho giáo viên, học viên, các phân hệ sử dụng backend API chung và cơ sở dữ liệu tập trung để trao đổi và quản lý dữ liệu. Đề tài tập trung xây dựng hệ thống trong phạm vi các nghiệp vụ và yêu cầu đã được khảo sát, phân tích của Trung tâm Ngoại ngữ, nhằm đáp ứng nhu cầu quản lý và hỗ trợ người dùng trong quá trình hoạt động của trung tâm.

 CHƯƠNG 1: KHẢO SÁT HỆ THỐNG
1.1. GIỚI THIỆU TỔNG QUAN

1.2. PHƯƠNG PHÁP THỰC HIỆN
Đề tài được thực hiện theo các giai đoạn gồm khảo sát và phân tích yêu cầu, phân tích hệ thống, thiết kế hệ thống, xây dựng phần mềm, kiểm thử và hoàn thiện sản phẩm. Trong giai đoạn đầu, nhóm tiến hành tìm hiểu các nghiệp vụ của trung tâm ngoại ngữ, xác định mục tiêu, phạm vi đề tài và thu thập yêu cầu từ các nhóm người dùng gồm quản trị viên, nhân viên học vụ, nhân viên tư vấn, giáo viên, trợ giảng, nhân viên kế toán và học viên.
Từ các yêu cầu đã thu thập, nhóm tiến hành mô hình hóa nghiệp vụ và phân tích hệ thống thông qua các sơ đồ Use Case nghiệp vụ, Use Case hệ thống và sơ đồ lớp phân tích. Sau đó, nhóm thực hiện thiết kế cơ sở dữ liệu, sơ đồ lớp thiết kế, phân quyền người dùng, giao diện và kiến trúc kết nối giữa các thành phần của hệ thống.
Trong giai đoạn xây dựng, hệ thống được phát triển thành hai phân hệ gồm ứng dụng WinForms dành cho quản trị viên và Website dành cho giáo viên, học viên. Hai phân hệ trao đổi dữ liệu thông qua backend API chung và sử dụng cơ sở dữ liệu tập trung. Sau khi hoàn thành các chức năng chính, nhóm tiến hành kiểm thử theo từng vai trò người dùng, kiểm tra các chức năng nghiệp vụ và khả năng đồng bộ dữ liệu giữa WinForms và Website. Kết quả kiểm thử được sử dụng để phát hiện lỗi, điều chỉnh hệ thống và hoàn thiện sản phẩm trước khi triển khai và báo cáo.
1.3. CÔNG NGHỆ SỬ DỤNG
Để xây dựng hệ thống quản lý Trung tâm Ngoại ngữ, nhóm dự kiến sử dụng các công nghệ phù hợp với từng thành phần của hệ thống nhằm đảm bảo khả năng phát triển, kết nối dữ liệu và mở rộng trong quá trình triển khai.
	Ứng dụng WinForms: Sử dụng ngôn ngữ C# trên nền tảng .NET để xây dựng ứng dụng dành cho quản trị viên. WinForms được sử dụng để phát triển các chức năng quản lý nội bộ như tài khoản người dùng, lớp học, lịch học, phân công giảng dạy, điểm số, học phí và báo cáo. 
	Backend API: Sử dụng ASP.NET Core Web API trên nền tảng .NET để xây dựng hệ thống API dùng chung. Backend chịu trách nhiệm xử lý nghiệp vụ, xác thực và phân quyền người dùng, truy xuất dữ liệu và cung cấp dữ liệu cho cả ứng dụng WinForms và Website. 
	Website: Sử dụng Next.js để xây dựng giao diện Website cho các nhóm người dùng như nhân viên học vụ, nhân viên tư vấn, giáo viên, trợ giảng, nhân viên kế toán và học viên. Next.js được sử dụng kết hợp với TypeScript/JavaScript để phát triển các chức năng và tương tác trên Website. 
	Giao diện Website: Sử dụng Tailwind CSS để thiết kế giao diện, giúp xây dựng các thành phần giao diện nhanh chóng, đồng nhất và hỗ trợ khả năng hiển thị tốt trên nhiều kích thước màn hình như máy tính, máy tính bảng và điện thoại. 
	Cơ sở dữ liệu: Sử dụng Microsoft SQL Server để lưu trữ tập trung các dữ liệu của hệ thống như tài khoản, nhân viên, học viên, chương trình đào tạo, khóa học, lớp học, lịch học, điểm danh, điểm số, học phí và thanh toán. 
	Kết nối dữ liệu: Ứng dụng WinForms và Website không truy cập trực tiếp cơ sở dữ liệu mà trao đổi dữ liệu thông qua ASP.NET Core Web API. Cách tổ chức này giúp đảm bảo dữ liệu giữa các phân hệ được xử lý thống nhất và thuận lợi cho việc phân quyền, bảo trì và mở rộng hệ thống.
1.4. KẾ HOẠCH VÀ PHÂN CÔNG THỰC HIỆN

1.5. KHẢO SÁT HIỆN TRẠNG
Trước khi tiến hành khảo sát nghiệp vụ theo từng vai trò, nhóm thực hiện khảo sát hiện trạng hoạt động của trung tâm ngoại ngữ nhằm nắm rõ cơ cấu tổ chức, quy trình xử lý công việc và các biểu mẫu, sổ sách đang được sử dụng trong thực tế, làm cơ sở cho việc xác định yêu cầu và thiết kế hệ thống.
1.5.1. Cơ cấu tổ chức
Trung tâm ngoại ngữ hiện được tổ chức theo các bộ phận chức năng gồm: Quản trị viên phụ trách quản lý chung; bộ phận tư vấn tiếp nhận và tư vấn học viên mới; bộ phận học vụ tổ chức chương trình đào tạo, lớp học và lịch học; đội ngũ giáo viên và trợ giảng trực tiếp giảng dạy; bộ phận tài chính phụ trách học phí và công nợ. Các bộ phận hiện phối hợp thủ công thông qua trao đổi trực tiếp hoặc các công cụ văn phòng thông thường như Excel, chưa có hệ thống quản lý tập trung.
1.5.2. Quy trình nghiệp vụ hiện tại
Quy trình tiếp nhận và xử lý học viên hiện tại diễn ra qua nhiều bước thủ công: nhân viên tư vấn ghi nhận thông tin học viên vào sổ hoặc file Excel, chuyển thông tin cho bộ phận học vụ để xếp lớp, thông báo cho giáo viên qua tin nhắn hoặc email, và bộ phận tài chính ghi nhận học phí riêng biệt trên một file khác. Việc thiếu liên kết dữ liệu giữa các bộ phận dễ dẫn đến sai sót, trùng lặp hoặc chậm trễ trong xử lý thông tin.
1.5.3. Biểu mẫu, sổ sách đang sử dụng
Trung tâm hiện sử dụng một số biểu mẫu và sổ sách như: phiếu đăng ký học/tư vấn, phiếu kết quả kiểm tra đầu vào, danh sách lớp học và điểm danh, biên nhận thu học phí, sổ theo dõi công nợ học viên. Các biểu mẫu này chủ yếu ở dạng giấy hoặc file Excel rời rạc, chưa được số hóa và liên kết với nhau, gây khó khăn cho việc tra cứu, tổng hợp và báo cáo.
1.6. KHẢO SÁT NGHIỆP VỤ QUẢN TRỊ HỆ THỐNG
Quản trị viên hệ thống chịu trách nhiệm quản lý tài khoản, quyền truy cập và các cấu hình chung của hệ thống. Vai trò này tập trung vào việc đảm bảo người dùng có thể truy cập đúng chức năng theo nhiệm vụ được phân công.
Các nghiệp vụ chính của Quản trị viên hệ thống bao gồm:
	Quản lý tài khoản người dùng: Tạo, cập nhật, khóa hoặc mở khóa tài khoản của nhân viên, giáo viên, trợ giảng và học viên. 
	Quản lý vai trò người dùng: Xác định và quản lý các vai trò như Quản trị viên, Nhân viên học vụ, Nhân viên tư vấn, Giáo viên, Trợ giảng, Nhân viên kế toán và Học viên. 
	Quản lý phân quyền: Thiết lập quyền truy cập chức năng và dữ liệu phù hợp với từng vai trò trong hệ thống. 
	Quản lý trạng thái tài khoản: Theo dõi trạng thái hoạt động của tài khoản và xử lý các trường hợp tài khoản bị khóa hoặc ngừng sử dụng. 
	Hỗ trợ tài khoản: Hỗ trợ đặt lại thông tin xác thực hoặc xử lý các vấn đề liên quan đến quyền truy cập khi cần thiết. 
	Quản lý cấu hình chung: Quản lý một số thông tin và thiết lập dùng chung của hệ thống phục vụ quá trình vận hành. 
	Quản lý học phí, thanh toán và công nợ của học viên: Theo dõi, đối soát và quản lý tổng thể học phí, thanh toán và công nợ của học viên trên ứng dụng WinForms; Nhân viên kế toán thực hiện các thao tác nghiệp vụ chi tiết hằng ngày trên Website. 
	Phân công giáo viên chính cho lớp học: Phân công hoặc thay đổi giáo viên chính phụ trách lớp học khi cần thiết trên ứng dụng WinForms. 
	Sao lưu và phục hồi dữ liệu: Thực hiện hoặc quản lý quá trình sao lưu, phục hồi dữ liệu khi có yêu cầu. 
	Theo dõi hoạt động hệ thống: Kiểm tra và giám sát các hoạt động quản trị cần thiết nhằm đảm bảo hệ thống vận hành ổn định. 
1.7. KHẢO SÁT NGHIỆP VỤ NHÂN VIÊN HỌC VỤ
Nhân viên học vụ chịu trách nhiệm tổ chức và quản lý các hoạt động đào tạo của trung tâm ngoại ngữ, bao gồm chương trình đào tạo, cấp độ, lớp học, giáo viên, học viên, lịch học và quá trình học tập.
Các nghiệp vụ chính của Nhân viên học vụ bao gồm:
	Quản lý chương trình đào tạo: Xây dựng và quản lý các chương trình như Preschool English, Primary English, Secondary English, Communication English và IELTS. 
	Quản lý cấp độ và khóa học: Xác định các cấp độ, nội dung học, thời lượng, yêu cầu đầu vào và mục tiêu đầu ra của từng khóa học. 
	Quản lý lớp học: Tạo và theo dõi các lớp với các thông tin như mã lớp, tên lớp, chương trình, cấp độ, ngày bắt đầu, ngày kết thúc, số lượng học viên tối đa và trạng thái lớp. 
	Xếp học viên vào lớp: Căn cứ vào chương trình đăng ký, độ tuổi, trình độ đầu vào và tình trạng lớp để sắp xếp học viên vào lớp phù hợp. 
	Phân công giáo viên và trợ giảng: Phân công giáo viên chính, giáo viên thay thế hoặc trợ giảng dựa trên chuyên môn, chứng chỉ, cấp độ có thể giảng dạy và lịch làm việc. Số lượng giáo viên và trợ giảng cho một lớp không cố định mà phụ thuộc vào chương trình và cấp độ: lớp thuộc chương trình Preschool English cần 1 giáo viên chính và 1 trợ giảng do sĩ số nhỏ và học viên nhỏ tuổi; lớp Primary English và Secondary English có thể phân công thêm trợ giảng khi sĩ số đông; lớp Communication English và IELTS dành cho học viên lớn tuổi thường chỉ cần 1 giáo viên chính. 
	Quản lý lịch học: Xây dựng lịch học cho từng lớp, bao gồm ngày học, giờ học, phòng học và thời gian áp dụng. 
	Quản lý phòng học: Sắp xếp phòng phù hợp với số lượng học viên và lịch học, đồng thời hạn chế tình trạng trùng phòng. 
	Quản lý ca học: Theo dõi từng ca học thực tế, giáo viên đứng lớp, trợ giảng, phòng học và trạng thái của ca học. 
	Theo dõi quá trình học tập: Theo dõi tình trạng điểm danh, kết quả học tập và các vấn đề phát sinh trong quá trình học của học viên. 
	Quản lý thay đổi lớp học: Xử lý các trường hợp chuyển lớp, bảo lưu, thay đổi giáo viên, thay đổi lịch học hoặc kết thúc lớp. 
	Phối hợp với các bộ phận khác: Trao đổi với nhân viên tư vấn, giáo viên, trợ giảng và nhân viên kế toán để đảm bảo quá trình đăng ký, xếp lớp, giảng dạy và theo dõi học viên được thống nhất.
1.8. KHẢO SÁT NGHIỆP VỤ NHÂN VIÊN TƯ VẤN
Nhân viên tư vấn là người trực tiếp tiếp nhận nhu cầu của học viên hoặc phụ huynh khi có nhu cầu tìm hiểu và đăng ký học tại trung tâm. Bộ phận này đóng vai trò kết nối giữa học viên với chương trình đào tạo và bộ phận học vụ.
Các nghiệp vụ chính của nhân viên tư vấn bao gồm:
	Tiếp nhận thông tin học viên: Ghi nhận các thông tin cơ bản của học viên như họ tên, ngày sinh, số điện thoại, email, địa chỉ và thông tin phụ huynh hoặc người liên hệ trong trường hợp học viên chưa đủ tuổi. 
	Xác định nhu cầu học tập: Trao đổi với học viên hoặc phụ huynh để xác định ngôn ngữ muốn học, mục tiêu học tập, độ tuổi, trình độ hiện tại, thời gian có thể tham gia học và các yêu cầu liên quan. 
	Tư vấn chương trình học: Căn cứ vào nhu cầu và thông tin của học viên để giới thiệu chương trình, cấp độ, thời lượng và hình thức học phù hợp. 
	Hướng dẫn kiểm tra đầu vào: Đối với các chương trình yêu cầu xác định trình độ, nhân viên tiếp nhận hướng dẫn học viên thực hiện kiểm tra hoặc đánh giá đầu vào trước khi xếp lớp. 
	Ghi nhận kết quả đầu vào: Tiếp nhận kết quả kiểm tra hoặc đánh giá trình độ để làm căn cứ tư vấn cấp độ và lớp học phù hợp. 
	Tư vấn lớp học: Cung cấp thông tin về các lớp đang hoặc sắp mở, bao gồm cấp độ, lịch học, thời gian, giáo viên, học phí và số lượng chỗ còn lại. 
	Tiếp nhận đăng ký học: Ghi nhận lựa chọn của học viên sau khi đã thống nhất chương trình hoặc lớp học phù hợp. 
	Tạo hồ sơ học viên: Hoàn thiện thông tin hồ sơ học viên để chuyển sang bộ phận học vụ và các bộ phận liên quan xử lý tiếp. 
	Phối hợp thu học phí: Chuyển thông tin đăng ký và khoản học phí cần thanh toán đến bộ phận kế toán để hoàn thành thủ tục nhập học. 
	Phối hợp cấp tài khoản học viên: Sau khi hoàn thành các thủ tục cần thiết, hệ thống tự động sinh tài khoản cho học viên ngay khi Nhân viên tư vấn hoàn tất hồ sơ đăng ký, không cần Quản trị viên hệ thống thao tác thủ công; thông tin đăng nhập được gửi cho học viên qua email hoặc số điện thoại đã đăng ký. 
	Chăm sóc và hỗ trợ ban đầu: Giải đáp các thắc mắc liên quan đến lớp học, lịch học, chương trình đào tạo và các thủ tục trước khi học viên bắt đầu tham gia lớp. 
	Quản lý chương trình ưu đãi: Áp dụng chính sách ưu đãi học phí khi học viên đăng ký và đóng học phí từ 2 khóa học trở lên trong cùng một lần đăng ký, mức ưu đãi đề xuất là giảm 10% trên tổng học phí các khóa học đăng ký kèm theo. 
	Đề xuất poster quảng cáo: Yêu cầu hoặc đề xuất bộ phận marketing thiết kế poster quảng cáo cho các chương trình ưu đãi, khóa học mới hoặc sự kiện của trung tâm. 
1.9. KHẢO SÁT NGHIỆP VỤ GIÁO VIÊN
Giáo viên là người trực tiếp thực hiện hoạt động giảng dạy và đánh giá quá trình học tập của học viên trong các lớp được phân công. Giáo viên đồng thời phối hợp với bộ phận học vụ để đảm bảo lớp học được tổ chức đúng kế hoạch.
Các nghiệp vụ chính của giáo viên bao gồm:
	Tiếp nhận lớp được phân công: Giáo viên nhận thông tin về lớp học, chương trình, cấp độ, lịch dạy, phòng học và danh sách học viên của lớp. 
	Theo dõi lịch giảng dạy: Giáo viên theo dõi ngày dạy, giờ dạy, phòng học và các thay đổi liên quan đến lịch của những lớp mình phụ trách. 
	Chuẩn bị nội dung giảng dạy: Căn cứ vào chương trình, cấp độ và tiến độ của lớp để chuẩn bị nội dung phù hợp cho từng ca học. 
	Thực hiện giảng dạy: Tổ chức hoạt động giảng dạy theo chương trình và kế hoạch của trung tâm. 
	Điểm danh học viên: Ghi nhận tình trạng tham gia của từng học viên trong mỗi ca học, chẳng hạn có mặt, vắng mặt hoặc các trạng thái khác theo quy định của trung tâm. 
	Theo dõi quá trình học tập: Quan sát và đánh giá mức độ tham gia, tiến bộ và những vấn đề học tập của từng học viên trong lớp. 
	Đánh giá kết quả học tập: Thực hiện các hoạt động đánh giá như bài tập, kiểm tra giữa kỳ, kiểm tra cuối kỳ hoặc các hình thức đánh giá khác theo chương trình. 
	Ghi nhận điểm và nhận xét: Ghi nhận kết quả đánh giá và nhận xét về tình hình học tập của học viên để phục vụ việc theo dõi và tổng hợp kết quả. 
	Phối hợp với bộ phận học vụ: Báo cáo các trường hợp học viên vắng nhiều, có vấn đề trong quá trình học, cần chuyển lớp hoặc có các tình huống phát sinh liên quan đến lớp học. 
	Thực hiện bàn giao khi thay đổi giáo viên: Trong trường hợp giáo viên không thể tiếp tục giảng dạy, giáo viên cần phối hợp với bộ phận học vụ để bàn giao tình trạng lớp, tiến độ và thông tin liên quan cho giáo viên thay thế. 
	Cập nhật thông tin chuyên môn: Giáo viên cần cung cấp và cập nhật các thông tin liên quan đến bằng cấp, chuyên ngành, chứng chỉ giảng dạy, ngoại ngữ có khả năng giảng dạy, cấp độ có thể phụ trách và kinh nghiệm chuyên môn để phục vụ việc phân công lớp.
1.10. KHẢO SÁT NGHIỆP VỤ TRỢ GIẢNG
Giáo viên là người trực tiếp thực hiện hoạt động giảng dạy và đánh giá quá trình học tập của học viên trong các lớp được phân công. Giáo viên đồng thời phối hợp với bộ phận học vụ để đảm bảo lớp học được tổ chức đúng kế hoạch.
Các nghiệp vụ chính của giáo viên bao gồm:
	Tiếp nhận lớp được phân công: Giáo viên nhận thông tin về lớp học, chương trình, cấp độ, lịch dạy, phòng học và danh sách học viên của lớp. 
	Theo dõi lịch giảng dạy: Giáo viên theo dõi ngày dạy, giờ dạy, phòng học và các thay đổi liên quan đến lịch của những lớp mình phụ trách. 
	Chuẩn bị nội dung giảng dạy: Căn cứ vào chương trình, cấp độ và tiến độ của lớp để chuẩn bị nội dung phù hợp cho từng ca học. 
	Thực hiện giảng dạy: Tổ chức hoạt động giảng dạy theo chương trình và kế hoạch của trung tâm. 
	Điểm danh học viên: Ghi nhận tình trạng tham gia của từng học viên trong mỗi ca học, chẳng hạn có mặt, vắng mặt hoặc các trạng thái khác theo quy định của trung tâm. 
	Theo dõi quá trình học tập: Quan sát và đánh giá mức độ tham gia, tiến bộ và những vấn đề học tập của từng học viên trong lớp. 
	Đánh giá kết quả học tập: Thực hiện các hoạt động đánh giá như bài tập, kiểm tra giữa kỳ, kiểm tra cuối kỳ hoặc các hình thức đánh giá khác theo chương trình. 
	Ghi nhận điểm và nhận xét: Ghi nhận kết quả đánh giá và nhận xét về tình hình học tập của học viên để phục vụ việc theo dõi và tổng hợp kết quả. 
	Phối hợp với bộ phận học vụ: Báo cáo các trường hợp học viên vắng nhiều, có vấn đề trong quá trình học, cần chuyển lớp hoặc có các tình huống phát sinh liên quan đến lớp học. 
	Thực hiện bàn giao khi thay đổi giáo viên: Trong trường hợp giáo viên không thể tiếp tục giảng dạy, giáo viên cần phối hợp với bộ phận học vụ để bàn giao tình trạng lớp, tiến độ và thông tin liên quan cho giáo viên thay thế. 
	Cập nhật thông tin chuyên môn: Giáo viên cần cung cấp và cập nhật các thông tin liên quan đến bằng cấp, chuyên ngành, chứng chỉ giảng dạy, ngoại ngữ có khả năng giảng dạy, cấp độ có thể phụ trách và kinh nghiệm chuyên môn để phục vụ việc phân công lớp.
1.11. KHẢO SÁT NGHIỆP VỤ NHÂN VIÊN KẾ TOÁN
Nhân viên kế toán chịu trách nhiệm tiếp nhận, ghi nhận và theo dõi các khoản học phí của học viên, đồng thời kiểm soát tình trạng thanh toán, công nợ và tổng hợp các số liệu tài chính liên quan đến hoạt động đào tạo của trung tâm.
Các nghiệp vụ chính của Nhân viên kế toán bao gồm:
	Tiếp nhận thông tin học phí cần thanh toán của học viên. 
	Kiểm tra số tiền học viên cần thanh toán. 
	Tiếp nhận và ghi nhận các khoản thanh toán học phí. 
	Cập nhật trạng thái thanh toán như chưa thanh toán, thanh toán một phần hoặc đã thanh toán. 
	Lập và cung cấp biên nhận hoặc xác nhận thanh toán cho học viên. 
	Theo dõi tình trạng học phí và công nợ của từng học viên. 
	Theo dõi các khoản chưa thanh toán hoặc thanh toán chưa đủ. 
	Kiểm tra và đối soát các giao dịch thanh toán đã ghi nhận. 
	Ghi nhận và xử lý các trường hợp hoàn tiền hoặc điều chỉnh học phí khi có yêu cầu hợp lệ. 
	Tổng hợp doanh thu học phí theo khoảng thời gian. 
	Tổng hợp số liệu liên quan đến học phí, thanh toán và công nợ. 
	Lập các báo cáo tài chính phục vụ công tác quản lý. 
	Phối hợp với nhân viên tư vấn và nhân viên học vụ khi cần xác nhận tình trạng học phí hoặc xử lý các vấn đề liên quan đến thanh toán.
1.12. KHẢO SÁT NGHIỆP VỤ HỌC VIÊN 
Học viên là người tham gia các chương trình và lớp học tại trung tâm, đồng thời là đối tượng chính sử dụng các dịch vụ đào tạo của trung tâm.
Các nghiệp vụ chính của học viên bao gồm:
	Cung cấp thông tin cá nhân phục vụ quá trình đăng ký học. 
	Đăng ký nhu cầu học tập và lựa chọn chương trình phù hợp. 
	Tiếp nhận tư vấn về chương trình, cấp độ, lớp học và lịch học. 
	Tiếp nhận thông tin lớp học sau khi được xếp lớp. 
	Theo dõi lịch học, phòng học và giáo viên phụ trách. 
	Tham gia các ca học theo lịch. 
	Thực hiện bài tập, kiểm tra và các hoạt động đánh giá. 
	Theo dõi điểm số và kết quả học tập. 
	Theo dõi tình trạng điểm danh và quá trình tham gia học. 
	Tiếp nhận thông báo từ giáo viên hoặc trung tâm. 
	Phản hồi các vấn đề liên quan đến lớp học, lịch học hoặc quá trình học tập.
	Đánh giá dịch vụ đào tạo và giảng dạy của trung tâm sau khi kết thúc khóa học hoặc lớp học.
	Tham gia khảo sát mức độ hài lòng về dịch vụ học tập của trung tâm khi có yêu cầu.
1.13. KẾT CHƯƠNG 

 CHƯƠNG 2: PHÂN TÍCH HỆ THỐNG
2.1. GIỚI THIỆU
2.2. YÊU CẦU HỆ THỐNG
2.2.1. Yêu cầu chức năng

2.2.2. Yêu cầu phi chức năng
Yêu cầu phi chức năng mô tả các tiêu chí về chất lượng, khả năng vận hành và mức độ đáp ứng của hệ thống, không trực tiếp thể hiện một nghiệp vụ cụ thể. Đối với hệ thống quản lý trung tâm ngoại ngữ, các yêu cầu phi chức năng bao gồm:
•	Bảo mật và kiểm soát truy cập: Hệ thống phải hỗ trợ xác thực người dùng bằng tài khoản và mật khẩu. Các thông tin nhạy cảm như mật khẩu, dữ liệu cá nhân, kết quả học tập và thông tin thanh toán cần được bảo vệ phù hợp. Quyền truy cập phải được kiểm soát theo các vai trò gồm Quản trị viên hệ thống, Nhân viên học vụ, Nhân viên tư vấn, Giáo viên, Trợ giảng, Nhân viên kế toán và Học viên. 
•	Hiệu năng: Hệ thống cần đảm bảo khả năng xử lý khi có nhiều người dùng truy cập và thao tác đồng thời. Theo yêu cầu của đề tài, hệ thống hướng đến khả năng hỗ trợ ít nhất 500 người dùng đồng thời. 
•	Khả năng mở rộng: Hệ thống cần được thiết kế theo hướng dễ mở rộng để có thể bổ sung thêm chương trình đào tạo, cấp độ, cơ sở hoặc chi nhánh, ngôn ngữ đào tạo khác, thanh toán trực tuyến, ứng dụng di động và các hình thức gửi thông báo qua email hoặc SMS trong tương lai. 
•	Khả năng bảo trì: Mã nguồn cần được tổ chức rõ ràng, phân chia theo các module chức năng, dễ đọc, dễ kiểm thử, nâng cấp và bảo trì. Hệ thống cần có tài liệu hướng dẫn cài đặt, vận hành và sử dụng. 
•	Đồng bộ dữ liệu: Dữ liệu giữa ứng dụng WinForms và Website phải được cập nhật chính xác và kịp thời thông qua Backend API chung, hạn chế tình trạng dữ liệu không nhất quán giữa các phân hệ. 
•	Tính toàn vẹn dữ liệu: Các thông tin như học viên, giáo viên, lớp học, lịch học, điểm danh, điểm số, học phí và thanh toán phải được lưu trữ nhất quán, hạn chế dữ liệu trùng lặp hoặc sai lệch. 
•	Tính thân thiện với người dùng: Giao diện cần đơn giản, trực quan và phù hợp với từng nhóm người dùng. Hệ thống hỗ trợ tiếng Việt và Website cần hiển thị phù hợp trên máy tính, máy tính bảng và điện thoại thông minh. 
•	Sao lưu và phục hồi dữ liệu: Hệ thống cần hỗ trợ sao lưu và phục hồi dữ liệu nhằm hạn chế nguy cơ mất dữ liệu khi xảy ra lỗi hoặc sự cố trong quá trình vận hành.
2.3. XÁC ĐỊNH CÁC MODULE CHÍNH
Dựa trên các nghiệp vụ đã khảo sát, hệ thống quản lý trung tâm ngoại ngữ được chia thành các module chính nhằm tổ chức chức năng rõ ràng và thuận tiện cho quá trình phân tích, thiết kế và phát triển.
•	Module Xác thực và phân quyền: Quản lý đăng nhập, đăng xuất, khôi phục mật khẩu, tài khoản người dùng, vai trò và quyền truy cập trong hệ thống. 
•	Module Quản lý nhân sự và người dùng: Quản lý thông tin của Quản trị viên hệ thống, Nhân viên học vụ, Nhân viên tư vấn, Giáo viên, Trợ giảng, Nhân viên kế toán và Học viên. 
•	Module Tiếp nhận và đăng ký học viên: Quản lý hồ sơ học viên, nhu cầu học tập, quá trình tư vấn, kiểm tra đầu vào và đăng ký khóa học. 
•	Module Quản lý chương trình đào tạo: Quản lý chương trình đào tạo, cấp độ, khóa học, yêu cầu đầu vào, mục tiêu đầu ra, nội dung, thời lượng và học phí của từng khóa học. 
•	Module Quản lý giáo viên và trợ giảng: Quản lý hồ sơ, chuyên môn, bằng cấp, chứng chỉ, kinh nghiệm và cấp độ có khả năng giảng dạy của giáo viên và trợ giảng. 
•	Module Quản lý lớp học và xếp lớp: Quản lý thông tin lớp học, khóa học, sĩ số, trạng thái lớp và việc xếp học viên vào lớp phù hợp. 
•	Module Phân công giảng dạy: Quản lý việc phân công giáo viên chính, giáo viên thay thế và trợ giảng cho từng lớp. 
•	Module Quản lý lịch học và phòng học: Quản lý lịch học định kỳ, thời gian, phòng học, ca học thực tế và các trường hợp thay đổi lịch. 
•	Module Quản lý điểm danh: Ghi nhận và theo dõi tình trạng tham gia học của học viên theo từng ca học. 
•	Module Quản lý kết quả học tập: Quản lý các loại điểm, điểm số, kết quả kiểm tra, nhận xét và quá trình học tập của học viên. 
•	Module Quản lý học phí và thanh toán: Quản lý học phí, giảm giá, ưu đãi khi đăng ký nhiều khóa học, học bổng, giao dịch thanh toán, biên nhận, công nợ, hoàn tiền và các khoản điều chỉnh. 
•	Module Đánh giá và khảo sát dịch vụ: Quản lý đánh giá của học viên về chất lượng đào tạo, giảng dạy và kết quả khảo sát mức độ hài lòng về dịch vụ học tập của trung tâm. 
•	Module Quản lý thông báo: Quản lý các thông báo liên quan đến lớp học, lịch học, kết quả học tập và các hoạt động của trung tâm. 
•	Module Thống kê và báo cáo: Tổng hợp số liệu về học viên, giáo viên, lớp học, điểm danh, kết quả học tập, học phí và thanh toán; hỗ trợ biểu đồ và xuất báo cáo Excel/PDF. 
•	Module Sao lưu và phục hồi dữ liệu: Hỗ trợ tạo bản sao lưu và phục hồi dữ liệu khi có sự cố hoặc yêu cầu quản trị.
2.4. XÁC ĐỊNH CÁC BẢNG DỮ LIỆU CHÍNH
Dựa trên các nghiệp vụ và module đã xác định, hệ thống dự kiến sử dụng các bảng dữ liệu chính sau:
•	NguoiDung: Lưu thông tin tài khoản đăng nhập của người dùng trong hệ thống, bao gồm tên đăng nhập, mật khẩu, email, vai trò và trạng thái tài khoản. 
•	VaiTro: Lưu thông tin các vai trò trong hệ thống, bao gồm Quản trị viên, Nhân viên tư vấn, Nhân viên học vụ, Kế toán, Giáo viên và Học viên. 
•	NhanVien: Lưu thông tin nhân viên của trung tâm như họ tên, ngày sinh, giới tính, số điện thoại, email, địa chỉ, chức vụ và trạng thái làm việc. 
•	GiaoVien: Lưu thông tin cá nhân và chuyên môn của giáo viên. Bảng có thuộc tính VaiTro để phân biệt Giáo viên chính thức và Giáo viên trợ giảng, thay vì tách thành hai bảng riêng. 
•	BangCap: Lưu thông tin bằng cấp của giáo viên như Cử nhân, Thạc sĩ, Tiến sĩ, chuyên ngành, cơ sở đào tạo, năm tốt nghiệp và số bằng. 
•	ChungChi: Lưu thông tin các chứng chỉ của giáo viên như TESOL, TEFL, CELTA hoặc các chứng chỉ ngoại ngữ như IELTS, TOEIC, JLPT, HSK, TOPIK,... 
•	HocVien: Lưu thông tin cá nhân, thông tin liên hệ, mục tiêu học tập và trạng thái học tập của học viên. 
•	PhuHuynh: Lưu thông tin phụ huynh hoặc người giám hộ của học viên, bao gồm họ tên, mối quan hệ, số điện thoại có thể để trống, email, địa chỉ và nghề nghiệp. 
•	HocVien_PhuHuynh: Lưu mối quan hệ giữa học viên và phụ huynh/người giám hộ. Một học viên có thể liên kết với nhiều phụ huynh và một phụ huynh có thể liên kết với nhiều học viên. Bảng lưu thêm mối quan hệ, xác định người liên hệ chính và ghi chú. 
•	ChuongTrinh: Lưu thông tin các chương trình đào tạo như Tiếng Anh Mầm non, Tiếng Anh Tiểu học, Tiếng Anh THCS, Tiếng Anh Giao tiếp và IELTS. 
•	CapDo: Lưu thông tin các cấp độ thuộc từng chương trình đào tạo, bao gồm mã cấp độ, tên cấp độ, thứ tự, điều kiện đầu vào, kết quả đầu ra và thời lượng dự kiến. 
•	KhoaHoc: Lưu thông tin các khóa học thuộc chương trình và cấp độ tương ứng, bao gồm số buổi, số giờ, học phí và trạng thái khóa học. 
•	KiemTraDauVao: Lưu thông tin kiểm tra và đánh giá trình độ tiếng Anh đầu vào của học viên, bao gồm điểm Nghe, Nói, Đọc, Viết, tổng điểm, cấp độ được đánh giá và nhận xét. 
•	LopHoc: Lưu thông tin các lớp học như mã lớp, tên lớp, khóa học, ngày bắt đầu, ngày kết thúc, sĩ số tối đa và trạng thái lớp. Mỗi lớp có một Nhân viên học vụ phụ trách. 
•	PhanCongGiangDay: Lưu thông tin phân công giáo viên cho từng lớp, bao gồm giáo viên, lớp học, ngày bắt đầu, ngày kết thúc và trạng thái phân công. Giáo viên được phân công theo lớp học, không cố định theo khóa học. 
•	DangKyHoc: Lưu thông tin đăng ký học của học viên, lớp học đăng ký, nhân viên tư vấn phụ trách, ưu đãi được áp dụng, học phí gốc, số tiền giảm và học phí sau giảm. 
•	PhongHoc: Lưu thông tin các phòng học của trung tâm như mã phòng, tên phòng, vị trí, sức chứa, thiết bị và trạng thái. 
•	LichHoc: Lưu lịch học định kỳ của từng lớp, bao gồm thứ trong tuần, giờ bắt đầu, giờ kết thúc, phòng học và thời gian áp dụng. 
•	CaHoc: Lưu thông tin từng ca học thực tế của lớp như ngày học, giờ học, phòng học, nội dung bài học và trạng thái ca học. 
•	DiemDanh: Lưu thông tin điểm danh của học viên theo từng ca học, bao gồm trạng thái điểm danh, thời gian điểm danh và người thực hiện điểm danh. 
•	LoaiDiem: Lưu các loại điểm của khóa học như bài tập, kiểm tra giữa kỳ, kiểm tra cuối kỳ hoặc các hình thức đánh giá khác, cùng với trọng số và điểm tối đa. 
•	DiemSo: Lưu điểm số, kết quả đánh giá và nhận xét của học viên, đồng thời lưu thông tin giáo viên thực hiện đánh giá. 
•	HocPhi: Lưu thông tin học phí phát sinh từ đăng ký học của học viên, bao gồm số tiền gốc, số tiền giảm, số tiền phải đóng, số tiền đã đóng, số tiền còn thiếu, hạn thanh toán và trạng thái học phí. 
•	ThanhToan: Lưu thông tin các giao dịch thanh toán học phí, bao gồm số tiền thanh toán, ngày thanh toán, phương thức thanh toán, kế toán thực hiện và mã giao dịch. 
•	BienNhan: Lưu thông tin biên nhận hoặc xác nhận thanh toán do kế toán lập sau khi học viên thực hiện thanh toán. 
•	UuDai: Lưu thông tin các chương trình ưu đãi học phí, bao gồm mã ưu đãi, tên ưu đãi, loại giảm giá, giá trị giảm, số khóa học tối thiểu, thời gian áp dụng và poster ưu đãi. Ví dụ: giảm 10% khi học viên đăng ký từ 2 khóa học trở lên. 
•	DichVu: Lưu thông tin các dịch vụ được cung cấp bởi trung tâm như tư vấn khóa học, đăng ký học, hỗ trợ học viên, dịch vụ học tập và các dịch vụ khác. 
•	PhanHoiDichVu: Lưu đánh giá và nhận xét của học viên đối với từng dịch vụ của trung tâm, bao gồm điểm đánh giá, nội dung phản hồi và thời gian phản hồi. 
•	KhaoSat: Lưu thông tin các chương trình khảo sát mức độ hài lòng của học viên về dịch vụ và chất lượng đào tạo. 
•	CauHoiKhaoSat: Lưu các câu hỏi thuộc từng khảo sát, bao gồm nội dung câu hỏi, loại câu hỏi, thứ tự hiển thị và trạng thái. 
•	PhanHoiKhaoSat: Lưu thông tin một lần tham gia khảo sát của học viên, bao gồm khảo sát thực hiện, học viên thực hiện, thời gian gửi và điểm đánh giá chung. 
•	CauTraLoiKhaoSat: Lưu câu trả lời của học viên cho từng câu hỏi trong một lần khảo sát, bao gồm giá trị đánh giá và nội dung trả lời. 
•	ThongBao: Lưu các thông báo được gửi trong hệ thống, bao gồm người gửi, người nhận, lớp học liên quan, nội dung, loại thông báo, thời gian gửi và trạng thái.
2.5. MÔ HÌNH HÓA NGHIỆP VỤ
2.6. XÁC ĐỊNH CÁC TÁC NHÂN
2.6.1. Quản trị viên hệ thống
2.6.2. Nhân viên học vụ
2.6.3. Nhân viên tư vấn
2.6.4. Giáo viên
2.6.5. Trợ giảng
2.6.6. Nhân viên kế toán
2.6.7. Học viên
2.7. MÔ HÌNH USE CASE NGHIỆP VỤ
2.7.1. Sơ đồ Use Case nghiệp vụ 
2.8. ĐẶC TẢ/ MÔ HÌNH HÓA QUY TRÌNH NGHIỆP VỤ 
2.8.1. Tiếp nhận và tư vấn học viên
Tên nghiệp vụ	Tiếp nhận và tư vấn học viên
Tác nhân tham gia	Nhân viên tư vấn, Học viên / Phụ huynh
Mục tiêu	Tiếp nhận nhu cầu học tập, thu thập thông tin và tư vấn chương trình, cấp độ hoặc khóa học phù hợp cho học viên.
Điều kiện bắt đầu	Học viên hoặc phụ huynh có nhu cầu tìm hiểu và đăng ký học tại trung tâm.
Luồng nghiệp vụ chính	1. Học viên hoặc phụ huynh liên hệ với trung tâm để tìm hiểu chương trình học. 
2. Nhân viên tư vấn tiếp nhận các thông tin cơ bản của học viên như họ tên, độ tuổi, thông tin liên hệ và thông tin phụ huynh nếu cần. 
3. Nhân viên tư vấn trao đổi để xác định mục tiêu học tập, trình độ hiện tại, thời gian có thể tham gia học và các yêu cầu liên quan. 
4. Nhân viên tư vấn giới thiệu các chương trình, cấp độ và khóa học phù hợp. 
5. Nhân viên tư vấn cung cấp thông tin về thời lượng học, lịch học dự kiến, học phí và các lớp đang hoặc sắp mở. 
6. Nếu chương trình yêu cầu đánh giá trình độ đầu vào, học viên được hướng dẫn thực hiện kiểm tra đầu vào.
7. Kết quả tư vấn và nhu cầu của học viên được ghi nhận để phục vụ bước đăng ký tiếp theo.
Kết quả	Học viên được tư vấn chương trình phù hợp và thông tin nhu cầu học tập được ghi nhận đầy đủ để tiếp tục kiểm tra đầu vào hoặc đăng ký khóa học.
Quy tắc nghiệp vụ	Thông tin tư vấn phải dựa trên độ tuổi, mục tiêu học tập, trình độ hiện tại và khả năng tham gia của học viên; học viên chưa đủ tuổi cần có thông tin phụ huynh hoặc người giám hộ khi cần thiết.

2.8.2. Kiểm tra và đánh giá đầu vào
Tên nghiệp vụ	Kiểm tra và đánh giá đầu vào
Tác nhân tham gia	Học viên, Nhân viên tư vấn, Giáo viên / Người đánh giá
Mục tiêu	Xác định trình độ tiếng Anh hiện tại của học viên để làm cơ sở lựa chọn cấp độ và khóa học phù hợp.
Điều kiện bắt đầu	Học viên có nhu cầu đăng ký chương trình yêu cầu xác định trình độ đầu vào hoặc chưa xác định được cấp độ phù hợp.
Luồng nghiệp vụ chính	1. Nhân viên tư vấn xác định học viên cần thực hiện kiểm tra đầu vào.
2. Học viên được hướng dẫn về thời gian, hình thức và nội dung kiểm tra. 
3. Học viên thực hiện bài kiểm tra hoặc hoạt động đánh giá theo quy định của trung tâm. 
4. Giáo viên hoặc người phụ trách đánh giá kết quả dựa trên các kỹ năng phù hợp như nghe, nói, đọc và viết. 
5. Kết quả kiểm tra được tổng hợp và sử dụng để xác định cấp độ phù hợp của học viên. 
6. Nhân viên tư vấn tiếp nhận kết quả đánh giá và thông báo cho học viên hoặc phụ huynh. 
7. Dựa trên kết quả đầu vào, nhân viên tư vấn tiếp tục đề xuất chương trình, cấp độ hoặc khóa học phù hợp.
Kết quả	Học viên có kết quả đánh giá đầu vào và được xác định cấp độ phù hợp để phục vụ việc đăng ký khóa học.
Quy tắc nghiệp vụ	Mỗi kết quả đánh giá phải gắn với đúng học viên và thời điểm kiểm tra; cấp độ được đề xuất phải dựa trên kết quả đánh giá và tiêu chí đầu vào của chương trình; chỉ người có trách nhiệm đánh giá mới được xác nhận kết quả đầu vào.

2.8.3. Đăng ký khóa học
Tên nghiệp vụ	Đăng ký khóa học
Tác nhân tham gia	Học viên / Phụ huynh, Nhân viên tư vấn, Nhân viên học vụ, Nhân viên kế toán
Mục tiêu	Ghi nhận việc học viên lựa chọn và đăng ký khóa học phù hợp, đồng thời chuẩn bị thông tin cần thiết cho việc thanh toán và xếp lớp.
Điều kiện bắt đầu	Học viên đã được tư vấn chương trình phù hợp và đã có kết quả đánh giá đầu vào nếu khóa học yêu cầu.
Luồng nghiệp vụ chính	1. Nhân viên tư vấn xác nhận chương trình, cấp độ và khóa học phù hợp với học viên. 
2. Nhân viên tư vấn cung cấp thông tin về khóa học, thời lượng, học phí và các lớp phù hợp đang hoặc sắp mở. 
3. Học viên hoặc phụ huynh lựa chọn khóa học muốn đăng ký.
4. Nhân viên tư vấn kiểm tra và hoàn thiện thông tin hồ sơ học viên. 
5. Việc đăng ký khóa học được ghi nhận với thông tin chương trình, cấp độ, khóa học và thời điểm đăng ký. 
6. Thông tin học phí được chuyển cho Nhân viên kế toán để thực hiện thủ tục thanh toán. 
7. Sau khi đáp ứng các điều kiện đăng ký cần thiết, thông tin được chuyển cho Nhân viên học vụ để thực hiện xếp học viên vào lớp phù hợp.
Kết quả	Thông tin đăng ký khóa học của học viên được ghi nhận và sẵn sàng cho quá trình thanh toán, xếp lớp và bắt đầu học.
Quy tắc nghiệp vụ	Học viên chỉ được đăng ký khóa học phù hợp với chương trình và cấp độ đã được tư vấn hoặc đánh giá; việc đăng ký phải gắn với đúng học viên; lớp được đề xuất phải còn khả năng tiếp nhận học viên; các nghĩa vụ học phí phải được thực hiện theo quy định của trung tâm.

2.8.4. Quản lý chương trình, cấp độ và khóa học
 
Tên nghiệp vụ	Quản lý chương trình, cấp độ và khóa học
Tác nhân tham gia	Nhân viên học vụ
Mục tiêu	Tổ chức và quản lý cấu trúc đào tạo của trung tâm, bao gồm chương trình, cấp độ và khóa học để làm cơ sở tư vấn, xếp lớp và tổ chức giảng dạy.
Điều kiện bắt đầu	Trung tâm có nhu cầu xây dựng mới, cập nhật hoặc ngừng áp dụng chương trình, cấp độ hoặc khóa học.
Luồng nghiệp vụ chính	1. Nhân viên học vụ xác định chương trình đào tạo theo nhóm đối tượng hoặc mục tiêu học tập. 
2. Với mỗi chương trình, nhân viên học vụ xác định các cấp độ tương ứng. 
3. Mỗi cấp độ được mô tả bằng yêu cầu đầu vào, mục tiêu đầu ra, thứ tự cấp độ và thời lượng dự kiến. 
4. Nhân viên học vụ xây dựng các khóa học thuộc từng chương trình và cấp độ. 
5. Khóa học được xác định về nội dung, số ca, tổng thời lượng, học phí và trạng thái áp dụng. 
6. Khi có thay đổi về nội dung đào tạo, nhân viên học vụ cập nhật thông tin chương trình, cấp độ hoặc khóa học. 
7. Các chương trình hoặc khóa học không còn áp dụng được chuyển sang trạng thái ngừng hoạt động.
Kết quả	Thông tin chương trình, cấp độ và khóa học được tổ chức rõ ràng và sẵn sàng phục vụ tư vấn, đăng ký, mở lớp và giảng dạy.
Quy tắc nghiệp vụ	Mỗi cấp độ phải thuộc một chương trình cụ thể; mỗi khóa học phải gắn với chương trình và cấp độ phù hợp; thứ tự cấp độ trong cùng chương trình phải được xác định rõ; khóa học đã phát sinh lớp hoặc dữ liệu liên quan không nên bị xóa hoàn toàn mà được chuyển trạng thái khi ngừng sử dụng.

2.8.5. Quản lý lớp học
 
Tên nghiệp vụ	Quản lý lớp học
Tác nhân tham gia	Nhân viên học vụ
Mục tiêu	Quản lý thông tin và tình trạng hoạt động của các lớp học, đảm bảo lớp học có đầy đủ thông tin và được theo dõi trong suốt quá trình hoạt động.
Điều kiện bắt đầu	Trung tâm có lớp học cần được quản lý hoặc cần cập nhật thông tin.
Luồng nghiệp vụ chính	1.	Nhân viên học vụ xác định lớp học cần quản lý. 
2.	Nhân viên học vụ theo dõi thông tin và số lượng học viên hiện tại của lớp. 
3.	Nhân viên học vụ kiểm tra tình trạng hoạt động của lớp. 
4.	Khi lớp hoàn thành, bị hủy hoặc không tiếp tục hoạt động, nhân viên học vụ cập nhật trạng thái lớp tương ứng. 
Thông tin lớp học được sử dụng để phục vụ việc xếp học viên, phân công giáo viên, lập lịch và theo dõi quá trình học tập.
Kết quả	Lớp học được tạo và quản lý đầy đủ, có trạng thái rõ ràng và sẵn sàng cho việc xếp học viên và tổ chức giảng dạy.
Quy tắc nghiệp vụ	Mỗi lớp phải thuộc một khóa học cụ thể; số lượng học viên không được vượt quá sĩ số tối đa; mã lớp phải đảm bảo khả năng phân biệt; lớp đã phát sinh học viên, lịch học hoặc dữ liệu học tập không nên bị xóa hoàn toàn mà cần chuyển sang trạng thái phù hợp.

2.8.6. Xếp học viên vào lớp
 
Tên nghiệp vụ	Xếp học viên vào lớp
Tác nhân tham gia	Nhân viên học vụ, Học viên
Mục tiêu	Sắp xếp học viên đã đăng ký khóa học vào lớp phù hợp với cấp độ, lịch học, độ tuổi và khả năng tiếp nhận của lớp.
Điều kiện bắt đầu	Học viên đã có hồ sơ đăng ký khóa học hợp lệ và đã xác định được chương trình, cấp độ phù hợp.
Luồng nghiệp vụ chính	1. Nhân viên học vụ tiếp nhận thông tin đăng ký của học viên. 
2. Nhân viên học vụ kiểm tra chương trình, cấp độ, khóa học, độ tuổi và lịch học mà học viên có thể tham gia. 
3. Nhân viên học vụ xem danh sách các lớp phù hợp đang mở hoặc sắp mở. 
4. Nhân viên học vụ kiểm tra sĩ số hiện tại và số lượng chỗ còn lại của từng lớp. 
5. Một lớp phù hợp được lựa chọn cho học viên. 
6. Nhân viên học vụ ghi nhận việc học viên được xếp vào lớp. 
7. Sĩ số hiện tại của lớp được cập nhật. 
8. Học viên được thông báo về lớp, lịch học, phòng học và các thông tin liên quan.
Kết quả	Học viên được xếp vào một lớp phù hợp và thông tin tham gia lớp được ghi nhận để phục vụ quá trình học tập.
Quy tắc nghiệp vụ	Học viên phải được xếp vào lớp phù hợp với khóa học và cấp độ đã đăng ký; số lượng học viên không được vượt quá sĩ số tối đa của lớp; một học viên không được có các đăng ký lớp trùng lặp không hợp lệ; việc chuyển lớp phải được ghi nhận và xử lý theo quy định của trung tâm.

2.8.7. Phân công giáo viên và trợ giảng
Tên nghiệp vụ	Phân công giáo viên và trợ giảng
Tác nhân tham gia	Nhân viên học vụ, Giáo viên, Trợ giảng
Mục tiêu	Phân công nhân sự giảng dạy phù hợp cho từng lớp dựa trên chuyên môn, trình độ, lịch làm việc và yêu cầu của lớp.
Điều kiện bắt đầu	Lớp học đã được tạo và có nhu cầu phân công giáo viên hoặc trợ giảng.
Luồng nghiệp vụ chính	1. Nhân viên học vụ xem thông tin lớp, khóa học và cấp độ cần giảng dạy. 
2. Nhân viên học vụ xem danh sách giáo viên và trợ giảng phù hợp. 
3. Nhân viên học vụ kiểm tra chuyên môn, trình độ, chứng chỉ, cấp độ có thể giảng dạy và lịch làm việc của từng người. 
4. Nhân viên học vụ lựa chọn giáo viên chính cho lớp. 5. Nếu lớp có nhu cầu hỗ trợ, nhân viên học vụ phân công thêm trợ giảng. 
6. Khi giáo viên chính không thể giảng dạy, nhân viên học vụ có thể phân công giáo viên thay thế. 
7. Thời gian và vai trò của từng phân công được ghi nhận. 
8. Giáo viên và trợ giảng nhận thông tin về lớp được phân công.
Kết quả	Lớp học có giáo viên và trợ giảng phù hợp, thông tin phân công được ghi nhận rõ ràng để phục vụ tổ chức giảng dạy.
Quy tắc nghiệp vụ	Giáo viên được phân công phải có chuyên môn và cấp độ giảng dạy phù hợp; không được phân công một người vào các lớp có lịch giảng dạy trùng nhau; mỗi lớp phải có giáo viên chính trong thời gian hoạt động; vai trò giáo viên chính, giáo viên thay thế và trợ giảng phải được xác định rõ.

2.8.8. Quản lý lịch học và phòng học
 
Tên nghiệp vụ	Quản lý lịch học và phòng học
Tác nhân tham gia	Nhân viên học vụ, Giáo viên, Trợ giảng, Học viên
Mục tiêu	Sắp xếp thời gian và phòng học cho từng lớp, đảm bảo lịch học phù hợp và hạn chế xung đột giữa lớp, phòng và nhân sự giảng dạy.
Điều kiện bắt đầu	Lớp học đã được tạo và đã có thông tin cần thiết để tổ chức lịch học.
Luồng nghiệp vụ chính	1. Nhân viên học vụ xác định ngày học, giờ bắt đầu, giờ kết thúc và chu kỳ học của lớp. 
2. Nhân viên học vụ lựa chọn phòng học có sức chứa phù hợp. 
3. Nhân viên học vụ kiểm tra lịch của phòng, giáo viên và trợ giảng để hạn chế trùng lịch. 
4. Lịch học được thiết lập theo từng ngày trong tuần và khoảng thời gian áp dụng. 
5. Giáo viên, trợ giảng và học viên được cung cấp thông tin lịch học và phòng học. 
6. Khi có thay đổi về thời gian, phòng học hoặc nhân sự, nhân viên học vụ thực hiện điều chỉnh lịch. 
7. Các bên liên quan được thông báo về nội dung thay đổi. 
8. Lịch học được sử dụng làm cơ sở tạo và theo dõi các ca học thực tế của lớp.
Kết quả	Lớp học có lịch học và phòng học rõ ràng, các bên liên quan có thể theo dõi và tham gia đúng thời gian, địa điểm.
Quy tắc nghiệp vụ	Không được bố trí nhiều lớp sử dụng cùng một phòng trong cùng thời gian; giáo viên hoặc trợ giảng không được có lịch phân công bị trùng; sức chứa phòng phải phù hợp với sĩ số lớp; mọi thay đổi lịch học cần được cập nhật và thông báo đến các bên liên quan.

2.8.9. Quản lý ca học
Tên nghiệp vụ	Quản lý ca học
Tác nhân tham gia	Nhân viên học vụ, Giáo viên, Trợ giảng
Mục tiêu	Theo dõi và quản lý từng ca học thực tế của lớp dựa trên lịch học đã được thiết lập.
Điều kiện bắt đầu	Lớp đã có lịch học và đến thời điểm tổ chức ca học.
Luồng nghiệp vụ chính	1. Từ lịch học của lớp, các ca học cụ thể được xác định theo ngày và thời gian tương ứng. 
2. Nhân viên học vụ theo dõi thông tin ca học gồm lớp, ngày học, thời gian, phòng học, giáo viên và trợ giảng. 
3. Giáo viên tiếp nhận thông tin ca học và thực hiện giảng dạy theo kế hoạch. 
4. Trong trường hợp có thay đổi, nhân viên học vụ có thể cập nhật giáo viên đứng lớp, trợ giảng, phòng học hoặc thời gian thực tế. 
5. Giáo viên ghi nhận nội dung đã giảng dạy và tình trạng của ca học. 
6. Sau khi kết thúc, ca học được cập nhật trạng thái phù hợp như đã hoàn thành, bị hủy hoặc hoãn.
Kết quả	Thông tin từng ca học thực tế được ghi nhận đầy đủ để phục vụ điểm danh, theo dõi tiến độ giảng dạy và quản lý lớp.
Quy tắc nghiệp vụ	Mỗi ca học phải thuộc một lớp cụ thể; thời gian và phòng học thực tế phải được xác định rõ; giáo viên đứng lớp phải là người được phân công hoặc được thay thế hợp lệ; ca học bị hủy hoặc hoãn phải được ghi nhận trạng thái và lý do khi cần thiết.

2.8.10. Điểm danh học viên
Tên nghiệp vụ	Điểm danh học viên
Tác nhân tham gia	Giáo viên, Trợ giảng, Học viên
Mục tiêu	Ghi nhận tình trạng tham gia của từng học viên trong mỗi ca học để theo dõi mức độ chuyên cần và quá trình học tập.
Điều kiện bắt đầu	Ca học đã được tổ chức và danh sách học viên của lớp đã được xác định.
Luồng nghiệp vụ chính	1. Giáo viên hoặc trợ giảng kiểm tra danh sách học viên của ca học. 
2. Tình trạng tham gia của từng học viên được xác định. 3. Giáo viên hoặc trợ giảng ghi nhận trạng thái như có mặt, vắng có phép, vắng không phép, đi trễ hoặc về sớm. 4. Ghi chú bổ sung được ghi nhận khi có tình huống cần giải thích. 
5. Kết quả điểm danh được lưu để theo dõi tình trạng chuyên cần của học viên.
6. Nhân viên học vụ và học viên có thể sử dụng thông tin điểm danh để theo dõi quá trình tham gia lớp.
Kết quả	Mỗi học viên có kết quả điểm danh tương ứng với từng ca học đã tham gia hoặc vắng mặt.
Quy tắc nghiệp vụ	Mỗi học viên chỉ có một kết quả điểm danh cho một ca học; kết quả phải gắn với đúng học viên và ca học; chỉ giáo viên, trợ giảng hoặc người được phân quyền mới được ghi nhận hoặc điều chỉnh điểm danh.

2.8.11. Quản lý kết quả học tập
Tên nghiệp vụ	Quản lý kết quả học tập
Tác nhân tham gia	Giáo viên, Nhân viên học vụ, Học viên
Mục tiêu	Ghi nhận, theo dõi và đánh giá kết quả học tập của học viên trong từng lớp và khóa học.
Điều kiện bắt đầu	Học viên đang tham gia lớp và có các hoạt động đánh giá như bài tập, kiểm tra giữa kỳ hoặc cuối kỳ.
Luồng nghiệp vụ chính	1. Giáo viên xác định hình thức đánh giá theo yêu cầu của khóa học. 
2. Học viên thực hiện bài tập hoặc bài kiểm tra. 
3. Giáo viên chấm và ghi nhận điểm của từng học viên. 4. Điểm được phân loại theo loại đánh giá như bài tập, giữa kỳ, cuối kỳ hoặc các hình thức khác. 
5. Giáo viên có thể ghi nhận nhận xét về kết quả và quá trình học tập của học viên. 
6. Giáo viên kiểm tra và xác nhận kết quả sau khi hoàn thành việc chấm điểm. 
7. Nhân viên học vụ theo dõi kết quả để phục vụ quản lý và đánh giá quá trình học tập. 
8. Học viên được xem điểm và nhận xét của bản thân.
Kết quả	Kết quả học tập của học viên được ghi nhận đầy đủ và có thể sử dụng cho việc đánh giá, tra cứu và thống kê.
Quy tắc nghiệp vụ	Mỗi điểm phải gắn với đúng học viên, lớp và loại điểm; giá trị điểm phải nằm trong giới hạn quy định; chỉ giáo viên hoặc người có quyền mới được nhập và điều chỉnh điểm; kết quả đã xác nhận chỉ được sửa khi có lý do hợp lệ.

2.8.12. Quản lý học phí và thanh toán
 
Tên nghiệp vụ	Quản lý học phí và thanh toán
Tác nhân tham gia	Nhân viên kế toán, Học viên / Phụ huynh, Nhân viên tư vấn
Mục tiêu	Theo dõi khoản học phí của học viên, ghi nhận thanh toán, quản lý công nợ và cung cấp xác nhận thanh toán.
Điều kiện bắt đầu	Học viên đã đăng ký khóa học hoặc phát sinh khoản học phí cần thanh toán.
Luồng nghiệp vụ chính	1. Nhân viên kế toán tiếp nhận thông tin đăng ký và xác định khoản học phí của học viên. 
2. Các khoản giảm giá, học bổng hoặc điều chỉnh hợp lệ được áp dụng nếu có. 
3. Nhân viên kế toán xác định tổng số tiền học viên cần thanh toán. 
4. Học viên hoặc phụ huynh thực hiện thanh toán bằng phương thức phù hợp. 
5. Nhân viên kế toán ghi nhận số tiền, ngày thanh toán, phương thức thanh toán và thông tin giao dịch. 
6. Hệ thống hoặc bộ phận tài chính cập nhật trạng thái học phí thành chưa thanh toán, thanh toán một phần hoặc đã thanh toán. 
7. Biên nhận hoặc xác nhận thanh toán được lập và cung cấp cho học viên. 
8. Nếu còn số tiền chưa thanh toán, khoản còn thiếu được tiếp tục theo dõi dưới dạng công nợ. 
9. Nhân viên kế toán thực hiện đối soát, điều chỉnh hoặc hoàn tiền khi có yêu cầu hợp lệ.
Kết quả	Thông tin học phí, thanh toán và công nợ của học viên được ghi nhận chính xác và có thể phục vụ tra cứu, đối soát và báo cáo.
Quy tắc nghiệp vụ	Mỗi khoản học phí phải gắn với đúng học viên và đăng ký học; mỗi giao dịch thanh toán phải có số tiền, thời gian và người ghi nhận rõ ràng; tổng số tiền đã thanh toán không được vượt quá nghĩa vụ học phí trừ trường hợp có điều chỉnh hợp lệ; việc hoàn tiền hoặc điều chỉnh phải được ghi nhận đầy đủ.

2.8.13. Quản lý thông báo
 
Tên nghiệp vụ	Quản lý thông báo
Tác nhân tham gia	Nhân viên học vụ, Giáo viên, Học viên, Trợ giảng
Mục tiêu	Cung cấp và truyền đạt kịp thời các thông tin liên quan đến lớp học, lịch học, quá trình học tập và các thay đổi cần thiết đến người dùng.
Điều kiện bắt đầu	Có thông tin cần truyền đạt đến một hoặc nhiều người dùng trong trung tâm.
Luồng nghiệp vụ chính	1. Nhân viên học vụ hoặc giáo viên xác định nội dung cần thông báo. 
2. Người gửi xác định đối tượng nhận thông báo như một học viên, toàn bộ lớp, giáo viên hoặc trợ giảng liên quan. 
3. Nội dung thông báo được soạn với tiêu đề, nội dung và loại thông báo phù hợp.
4. Thông báo được gửi đến các đối tượng đã xác định. 5. Người nhận có thể xem thông báo liên quan đến tài khoản hoặc lớp của mình. 
6. Trong trường hợp có thay đổi lịch học, phòng học hoặc giáo viên, thông báo được gửi đến các bên bị ảnh hưởng. 
7. Trạng thái gửi hoặc tiếp nhận thông báo được ghi nhận khi cần thiết.
Kết quả	Thông tin cần thiết được truyền đạt đến đúng đối tượng để hỗ trợ quá trình học tập và vận hành lớp học.
Quy tắc nghiệp vụ	Thông báo phải xác định rõ người gửi và đối tượng nhận; người dùng chỉ được gửi thông báo trong phạm vi quyền hạn của mình; thông báo liên quan đến một lớp phải được gửi đến đúng các thành viên liên quan; nội dung thông báo không được bị thay đổi sai lệch sau khi đã gửi nếu không có cơ chế cập nhật phù hợp.

2.9. MÔ HÌNH USE CASE HỆ THỐNG
2.9.1. Sơ đồ Use Case hệ thống 
2.10. ĐẶC TẢ USE CASE HỆ THỐNG
2.10.1. Đăng nhập
Tên Use Case	Đăng nhập
Tác nhân	Tất cả người dùng có tài khoản trong hệ thống
Mục tiêu	Cho phép người dùng xác thực tài khoản và truy cập vào các chức năng phù hợp với vai trò được phân quyền.
Tiền điều kiện	Người dùng đã có tài khoản hợp lệ và tài khoản đang ở trạng thái hoạt động.
Hậu điều kiện	Người dùng đăng nhập thành công và được chuyển đến giao diện phù hợp với vai trò của mình.
Luồng chính	1. Người dùng truy cập chức năng đăng nhập. 
2. Hệ thống hiển thị giao diện đăng nhập. 
3. Người dùng nhập tên đăng nhập hoặc email và mật khẩu. 
4. Người dùng gửi yêu cầu đăng nhập. 
5. Hệ thống kiểm tra thông tin tài khoản và mật khẩu.
6. Hệ thống kiểm tra trạng thái tài khoản. 
7. Hệ thống xác định vai trò và quyền của người dùng.
8. Hệ thống cho phép truy cập và chuyển người dùng đến giao diện tương ứng.
Luồng thay thế	Người dùng nhập thiếu tên đăng nhập, email hoặc mật khẩu; hệ thống yêu cầu nhập đầy đủ thông tin trước khi tiếp tục.
Luồng ngoại lệ	Tên đăng nhập, email hoặc mật khẩu không chính xác; tài khoản bị khóa hoặc ngừng hoạt động; hệ thống từ chối đăng nhập và hiển thị thông báo phù hợp.

2.10.2. Quên mật khẩu
Tên Use Case	Quên mật khẩu
Tác nhân	Người dùng có tài khoản trong hệ thống
Mục tiêu	Cho phép người dùng khôi phục quyền truy cập khi quên mật khẩu.
Tiền điều kiện	Người dùng đã có tài khoản và tài khoản có địa chỉ email hợp lệ.
Hậu điều kiện	Người dùng thiết lập được mật khẩu mới và có thể sử dụng mật khẩu mới để đăng nhập.
Luồng chính	1. Người dùng chọn chức năng Quên mật khẩu tại màn hình đăng nhập. 
2. Hệ thống yêu cầu nhập email đã đăng ký. 
3. Người dùng nhập email và gửi yêu cầu. 
4. Hệ thống kiểm tra email có tồn tại hay không.
5. Hệ thống tạo liên kết hoặc mã đặt lại mật khẩu có thời hạn. 
6. Hệ thống gửi thông tin khôi phục đến email của người dùng. 
7. Người dùng truy cập liên kết hoặc nhập mã xác nhận. 
8. Hệ thống yêu cầu nhập mật khẩu mới. 
9. Người dùng nhập và xác nhận mật khẩu mới. 
10. Hệ thống kiểm tra tính hợp lệ và cập nhật mật khẩu. 
11. Hệ thống thông báo đặt lại mật khẩu thành công.
Luồng thay thế	Người dùng nhập thiếu email hoặc email không đúng định dạng; hệ thống yêu cầu nhập lại.
Luồng ngoại lệ	Email không tồn tại; liên kết hoặc mã khôi phục đã hết hạn hoặc không hợp lệ; hệ thống từ chối yêu cầu và hiển thị thông báo phù hợp.

2.10.3. Quản lý tài khoản và phân quyền
Tên Use Case	Quản lý tài khoản và phân quyền
Tác nhân	Quản trị viên hệ thống
Mục tiêu	Cho phép quản trị viên tạo và quản lý tài khoản người dùng, đồng thời gán vai trò và quyền truy cập phù hợp cho từng tài khoản.
Tiền điều kiện	Quản trị viên đã đăng nhập và có quyền quản lý tài khoản, vai trò và phân quyền.
Hậu điều kiện	Thông tin tài khoản và quyền truy cập của người dùng được tạo mới hoặc cập nhật theo thao tác của quản trị viên.
Luồng chính	1. Quản trị viên truy cập chức năng Quản lý tài khoản và phân quyền. 
2. Hệ thống hiển thị danh sách tài khoản người dùng. 3. Quản trị viên có thể tìm kiếm hoặc lọc tài khoản theo tên, email, vai trò hoặc trạng thái. 
4. Quản trị viên chọn tạo mới hoặc chỉnh sửa tài khoản.
5. Hệ thống hiển thị biểu mẫu thông tin tài khoản. 
6. Quản trị viên nhập hoặc cập nhật thông tin người dùng.
7. Quản trị viên chọn vai trò phù hợp như Admin, Academic Staff, Consultant, Teacher, Teaching Assistant, Accountant hoặc Student. 
8. Hệ thống hiển thị các quyền tương ứng với vai trò được chọn. 
9. Quản trị viên xác nhận thông tin. 
10. Hệ thống kiểm tra tính hợp lệ của dữ liệu. 
11. Hệ thống lưu tài khoản và thông tin phân quyền. 
12. Hệ thống cập nhật danh sách và thông báo thao tác thành công.
Luồng thay thế	Quản trị viên có thể khóa hoặc mở khóa tài khoản; thay đổi vai trò; đặt lại mật khẩu; hoặc hủy thao tác trước khi lưu.
Luồng ngoại lệ	Username hoặc email bị trùng; vai trò không hợp lệ; dữ liệu bắt buộc bị thiếu; tài khoản đang có dữ liệu liên quan không thể xóa trực tiếp; hệ thống từ chối thao tác và hiển thị thông báo lỗi.

2.10.4. Quản lý chương trình, cấp độ và khóa học
Tên Use Case	Quản lý chương trình, cấp độ và khóa học
Tác nhân	Nhân viên học vụ
Mục tiêu	Cho phép nhân viên học vụ tạo, cập nhật và quản lý các chương trình đào tạo, cấp độ và khóa học của trung tâm.
Tiền điều kiện	Nhân viên học vụ đã đăng nhập và có quyền quản lý chương trình đào tạo.
Hậu điều kiện	Thông tin chương trình, cấp độ và khóa học được tạo mới hoặc cập nhật và có thể sử dụng cho quá trình tư vấn, đăng ký và mở lớp.
Luồng chính	1. Nhân viên học vụ truy cập chức năng quản lý chương trình đào tạo. 
2. Hệ thống hiển thị danh sách chương trình hiện có. 
3. Nhân viên học vụ chọn tạo mới hoặc chỉnh sửa chương trình. 
4. Nhân viên học vụ nhập tên chương trình, nhóm đối tượng, độ tuổi phù hợp, mục tiêu và mô tả. 
5. Hệ thống kiểm tra và lưu thông tin chương trình. 
6. Nhân viên học vụ chọn một chương trình để quản lý cấp độ. 
7. Nhân viên học vụ tạo hoặc cập nhật cấp độ gồm tên cấp độ, yêu cầu đầu vào, mục tiêu đầu ra, thời lượng dự kiến và thứ tự cấp độ. 
8. Nhân viên học vụ chọn cấp độ để tạo hoặc cập nhật khóa học. 
9. Nhân viên học vụ nhập tên khóa học, nội dung, số ca, tổng số giờ, học phí và trạng thái. 
10. Hệ thống kiểm tra mối liên kết giữa chương trình, cấp độ và khóa học. 
11. Hệ thống lưu thông tin và cập nhật danh sách.
Luồng thay thế	Nhân viên học vụ có thể tìm kiếm, lọc hoặc chuyển chương trình, cấp độ hoặc khóa học sang trạng thái ngừng hoạt động thay vì xóa.
Luồng ngoại lệ	Thông tin bắt buộc bị thiếu; mã chương trình, cấp độ hoặc khóa học bị trùng; cấp độ không thuộc chương trình hợp lệ; dữ liệu đang được sử dụng bởi lớp học nên không thể xóa; hệ thống thông báo lỗi.

2.10.5. Quản lý hồ sơ học viên
Tên Use Case	Quản lý hồ sơ học viên
Tác nhân	Nhân viên tư vấn, Nhân viên học vụ
Mục tiêu	Cho phép lưu trữ, tra cứu và cập nhật thông tin cá nhân, thông tin liên hệ, nhu cầu học tập và thông tin phụ huynh hoặc người giám hộ của học viên.
Tiền điều kiện	Nhân viên tư vấn hoặc nhân viên học vụ đã đăng nhập và có quyền quản lý hồ sơ học viên.
Hậu điều kiện	Hồ sơ học viên được tạo mới hoặc cập nhật và có thể sử dụng cho quá trình tư vấn, kiểm tra đầu vào, đăng ký và xếp lớp.
Luồng chính	1. Người dùng truy cập chức năng Quản lý hồ sơ học viên. 2. Hệ thống hiển thị danh sách học viên hiện có. 
3. Người dùng có thể tìm kiếm học viên theo mã, họ tên, số điện thoại hoặc email. 
4. Người dùng chọn tạo hồ sơ mới hoặc chỉnh sửa hồ sơ hiện có. 
5. Hệ thống hiển thị biểu mẫu thông tin học viên. 
6. Người dùng nhập hoặc cập nhật các thông tin gồm họ tên, ngày sinh, giới tính, số điện thoại, email, địa chỉ và mục tiêu học tập. 
7. Nếu học viên chưa đủ tuổi, người dùng nhập thông tin phụ huynh hoặc người giám hộ. 
8. Người dùng có thể ghi nhận trình độ đầu vào, trình độ hiện tại hoặc các thông tin liên quan đến quá trình tư vấn. 9. Hệ thống kiểm tra tính hợp lệ và trùng lặp của dữ liệu. 10. Hệ thống lưu hồ sơ học viên. 
11. Hệ thống cập nhật danh sách và thông báo thao tác thành công.
Luồng thay thế	Người dùng có thể xem chi tiết hồ sơ, cập nhật thông tin liên hệ, thêm hoặc thay đổi thông tin phụ huynh hoặc người giám hộ, hoặc hủy thao tác trước khi lưu.
Luồng ngoại lệ	Thông tin bắt buộc bị thiếu; email hoặc số điện thoại không hợp lệ; mã học viên bị trùng; dữ liệu không thể cập nhật do không còn quyền truy cập; hệ thống thông báo lỗi và không lưu thay đổi.

2.10.6. Quản lý kiểm tra đầu vào
Tên Use Case	Quản lý kiểm tra đầu vào
Tác nhân	Nhân viên tư vấn, Giáo viên 
Mục tiêu	Cho phép tạo, theo dõi và ghi nhận kết quả kiểm tra đầu vào của học viên để xác định cấp độ phù hợp.
Tiền điều kiện	Nhân viên tư vấn hoặc người đánh giá đã đăng nhập; học viên đã có hồ sơ trong hệ thống.
Hậu điều kiện	Kết quả kiểm tra đầu vào của học viên được ghi nhận và cấp độ phù hợp được xác định để phục vụ tư vấn và đăng ký khóa học.
Luồng chính	1. Nhân viên tư vấn truy cập chức năng Quản lý kiểm tra đầu vào. 
2. Hệ thống hiển thị danh sách các lượt kiểm tra đầu vào. 
3. Nhân viên tư vấn chọn học viên cần kiểm tra. 
4. Nhân viên tư vấn tạo thông tin kiểm tra gồm ngày kiểm tra và người phụ trách đánh giá. 
5. Hệ thống lưu thông tin kiểm tra. 
6. Giáo viên hoặc người đánh giá truy cập lượt kiểm tra được phân công. 
7. Sau khi học viên hoàn thành kiểm tra, người đánh giá nhập điểm các kỹ năng như Listening, Speaking, Reading và Writing. 
8. Hệ thống tính hoặc ghi nhận tổng điểm theo quy định. 
9. Người đánh giá lựa chọn cấp độ phù hợp và nhập nhận xét. 
10. Người đánh giá xác nhận kết quả. 
11. Hệ thống lưu kết quả và cập nhật trạng thái lượt kiểm tra. 
12. Nhân viên tư vấn có thể xem kết quả để tiếp tục tư vấn khóa học phù hợp.
Luồng thay thế	Người đánh giá có thể lưu tạm kết quả và hoàn thiện sau; nhân viên tư vấn có thể thay đổi thời gian kiểm tra trước khi kết quả được xác nhận.
Luồng ngoại lệ	Học viên không tồn tại; người đánh giá không có quyền; điểm nhập vượt ngoài giới hạn; cấp độ được chọn không hợp lệ; hệ thống thông báo lỗi và không xác nhận kết quả.

2.10.7. Đăng ký khóa học
Tên Use Case	Đăng ký khóa học
Tác nhân	Nhân viên tư vấn
Mục tiêu	Cho phép nhân viên tư vấn ghi nhận việc học viên đăng ký một khóa học phù hợp sau quá trình tư vấn hoặc đánh giá đầu vào.
Tiền điều kiện	Nhân viên tư vấn đã đăng nhập; học viên đã có hồ sơ; khóa học cần đăng ký đang hoạt động; học viên đã có kết quả kiểm tra đầu vào nếu khóa học yêu cầu.
Hậu điều kiện	Thông tin đăng ký khóa học của học viên được ghi nhận và có thể sử dụng cho quá trình thanh toán và xếp lớp.
Luồng chính	1. Nhân viên tư vấn truy cập hồ sơ học viên. 
2. Nhân viên tư vấn chọn chức năng Đăng ký khóa học. 
3. Hệ thống hiển thị danh sách chương trình, cấp độ và khóa học đang hoạt động. 
4. Nhân viên tư vấn lựa chọn khóa học phù hợp với học viên. 
5. Hệ thống hiển thị thông tin khóa học gồm cấp độ, thời lượng, số ca và học phí.
6. Nhân viên tư vấn xác nhận lựa chọn của học viên. 
7. Nhân viên tư vấn nhập các thông tin đăng ký cần thiết. 8. Hệ thống kiểm tra điều kiện đầu vào và trạng thái khóa học.
9. Hệ thống tạo thông tin đăng ký khóa học. 
10. Hệ thống xác định khoản học phí tương ứng. 
11. Hệ thống thông báo đăng ký thành công và chuyển trạng thái hồ sơ sang chờ thanh toán hoặc chờ xếp lớp theo quy định.
Luồng thay thế	Nhân viên tư vấn có thể thay đổi khóa học trước khi xác nhận; học viên có thể chưa chọn lớp cụ thể và chỉ đăng ký khóa học trước.
Luồng ngoại lệ	Khóa học đã ngừng hoạt động; học viên không đáp ứng điều kiện đầu vào; thông tin đăng ký không hợp lệ hoặc bị trùng; hệ thống từ chối tạo đăng ký và hiển thị thông báo.

2.10.8. Quản lý lớp học
Tên Use Case	Quản lý lớp học
Tác nhân	Nhân viên học vụ
Mục tiêu	Cho phép nhân viên học vụ tạo, cập nhật và quản lý các lớp học được mở từ các khóa học của trung tâm.
Tiền điều kiện	Nhân viên học vụ đã đăng nhập và có quyền quản lý lớp học; khóa học cần mở lớp đã tồn tại và đang hoạt động.
Hậu điều kiện	Thông tin lớp học được tạo mới hoặc cập nhật và có thể sử dụng cho việc xếp học viên, phân công giảng dạy và lập lịch học.
Luồng chính	1. Nhân viên học vụ truy cập chức năng Quản lý lớp học. 
2. Hệ thống hiển thị danh sách lớp hiện có. 
3. Nhân viên học vụ có thể tìm kiếm hoặc lọc lớp theo khóa học, trạng thái hoặc thời gian. 
4. Nhân viên học vụ chọn tạo lớp mới. 
5. Hệ thống hiển thị biểu mẫu thông tin lớp. 
6. Nhân viên học vụ chọn khóa học và nhập mã lớp, tên lớp, ngày bắt đầu, ngày kết thúc dự kiến, sĩ số tối đa và trạng thái lớp. 
7. Hệ thống kiểm tra tính hợp lệ của dữ liệu và mã lớp. 
8. Hệ thống lưu thông tin lớp. 
9. Khi cần thay đổi, nhân viên học vụ chọn lớp và cập nhật các thông tin được phép chỉnh sửa. 
10. Khi lớp hoàn thành, bị hủy hoặc ngừng hoạt động, nhân viên học vụ cập nhật trạng thái lớp. 
11. Hệ thống lưu thay đổi và cập nhật danh sách lớp.
Luồng thay thế	Nhân viên học vụ có thể hủy thao tác trước khi lưu; có thể chuyển lớp sang trạng thái Planned, Active, Completed, Cancelled hoặc Inactive tùy quy định.
Luồng ngoại lệ	Mã lớp bị trùng; khóa học không tồn tại hoặc đã ngừng hoạt động; ngày kết thúc nhỏ hơn ngày bắt đầu; sĩ số tối đa không hợp lệ; lớp đã có dữ liệu liên quan nên không thể xóa trực tiếp; hệ thống thông báo lỗi.

2.10.9. Xếp học viên vào lớp
Tên Use Case	Xếp học viên vào lớp
Tác nhân	Nhân viên học vụ
Mục tiêu	Cho phép nhân viên học vụ đưa học viên đã đăng ký khóa học vào lớp phù hợp dựa trên khóa học, cấp độ, lịch học và sĩ số lớp.
Tiền điều kiện	Nhân viên học vụ đã đăng nhập; học viên đã có đăng ký khóa học hợp lệ; lớp phù hợp đã được tạo.
Hậu điều kiện	Học viên được ghi nhận là thành viên của lớp và sĩ số lớp được cập nhật.
Luồng chính	1. Nhân viên học vụ truy cập chức năng Xếp học viên vào lớp. 
2. Hệ thống hiển thị danh sách học viên đang chờ xếp lớp. 
3. Nhân viên học vụ chọn một học viên. 
4. Hệ thống hiển thị thông tin đăng ký gồm khóa học, cấp độ, lịch mong muốn và các thông tin liên quan. 
5. Hệ thống hiển thị danh sách các lớp phù hợp.
6. Nhân viên học vụ xem thông tin lớp gồm lịch học, giáo viên, sĩ số hiện tại và sĩ số tối đa. 
7. Nhân viên học vụ chọn lớp phù hợp. 
8. Hệ thống kiểm tra học viên chưa có đăng ký lớp bị trùng và lớp vẫn còn chỗ. 
9. Nhân viên học vụ xác nhận xếp lớp. 
10. Hệ thống tạo thông tin Enrollment giữa học viên và lớp. 
11. Hệ thống cập nhật sĩ số lớp. 
12. Hệ thống thông báo xếp lớp thành công.
Luồng thay thế	Nếu chưa có lớp phù hợp hoặc lớp đã đầy, học viên có thể được giữ ở trạng thái chờ xếp lớp; nhân viên học vụ có thể chuyển học viên sang lớp khác nếu có yêu cầu hợp lệ.
Luồng ngoại lệ	Lớp đã đủ sĩ số; lớp không còn hoạt động; học viên đã thuộc lớp đó; lịch học bị trùng với lớp khác của học viên; hệ thống từ chối xếp lớp và hiển thị thông báo.

2.10.10. Phân công giáo viên và trợ giảng
Tên Use Case	Phân công giáo viên và trợ giảng
Tác nhân	Nhân viên học vụ
Mục tiêu	Cho phép nhân viên học vụ phân công giáo viên chính, giáo viên thay thế hoặc trợ giảng cho từng lớp.
Tiền điều kiện	Nhân viên học vụ đã đăng nhập; lớp học đã tồn tại; giáo viên và trợ giảng cần phân công đã có hồ sơ và đang ở trạng thái hoạt động.
Hậu điều kiện	Thông tin phân công giảng dạy của lớp được ghi nhận hoặc cập nhật, và giáo viên hoặc trợ giảng có thể xem lớp được phân công.
Luồng chính	1. Nhân viên học vụ truy cập chức năng Phân công giảng dạy. 
2. Hệ thống hiển thị danh sách các lớp. 
3. Nhân viên học vụ chọn lớp cần phân công. 
4. Hệ thống hiển thị thông tin khóa học, cấp độ, lịch học và các phân công hiện tại của lớp. 
5. Nhân viên học vụ chọn chức năng thêm phân công. 
6. Hệ thống hiển thị danh sách giáo viên và trợ giảng phù hợp. 
7. Nhân viên học vụ chọn người cần phân công. 
8. Nhân viên học vụ chọn vai trò giảng dạy như Main Teacher, Substitute Teacher hoặc Teaching Assistant. 
9. Nhân viên học vụ nhập thời gian bắt đầu và kết thúc của phân công nếu cần. 
10. Hệ thống kiểm tra trạng thái nhân sự, khả năng giảng dạy và xung đột lịch. 
11. Nhân viên học vụ xác nhận phân công. 
12. Hệ thống lưu thông tin Teacher Assignment. 
13. Hệ thống cập nhật danh sách phân công và thông báo thành công.
Luồng thay thế	Nhân viên học vụ có thể thay đổi giáo viên chính, thêm hoặc thay đổi trợ giảng, kết thúc một phân công hiện tại hoặc chỉ định giáo viên thay thế trong một khoảng thời gian nhất định.
Luồng ngoại lệ	Giáo viên hoặc trợ giảng không tồn tại hoặc không hoạt động; không phù hợp với cấp độ cần giảng dạy; bị trùng lịch với lớp khác; lớp không tồn tại hoặc đã kết thúc; hệ thống từ chối phân công và hiển thị thông báo.

2.10.11. Quản lý lịch học và phòng học
Tên Use Case	Quản lý lịch học và phòng học
Tác nhân	Nhân viên học vụ
Mục tiêu	Cho phép nhân viên học vụ tạo, cập nhật và quản lý lịch học, phòng học của từng lớp, đồng thời hạn chế xung đột về thời gian và phòng học.
Tiền điều kiện	Nhân viên học vụ đã đăng nhập; lớp học đã tồn tại; phòng học đã được khai báo trong hệ thống.
Hậu điều kiện	Lịch học và phòng học của lớp được tạo mới hoặc cập nhật và có thể được giáo viên, trợ giảng và học viên tra cứu.
Luồng chính	1. Nhân viên học vụ truy cập chức năng Quản lý lịch học. 2. Hệ thống hiển thị danh sách các lớp và lịch học hiện có. 3. Nhân viên học vụ chọn lớp cần thiết lập hoặc điều chỉnh lịch. 
4. Hệ thống hiển thị thông tin lớp và các lịch học hiện tại. 5. Nhân viên học vụ chọn ngày trong tuần, giờ bắt đầu, giờ kết thúc và khoảng thời gian áp dụng. 
6. Nhân viên học vụ chọn phòng học phù hợp. 
7. Hệ thống kiểm tra sức chứa phòng và các lịch đã tồn tại. 8. Hệ thống kiểm tra xung đột phòng học, lịch của giáo viên và trợ giảng được phân công. 
9. Nhân viên học vụ xác nhận lịch học. 
10. Hệ thống lưu thông tin lịch và phòng học. 
11. Hệ thống cập nhật lịch của lớp và thông báo thao tác thành công.
Luồng thay thế	Nhân viên học vụ có thể chỉnh sửa ngày học, giờ học hoặc phòng học; có thể tạo lịch lặp theo tuần; có thể ngừng áp dụng một lịch cũ và tạo lịch mới khi lớp thay đổi lịch.
Luồng ngoại lệ	Phòng học đã được sử dụng trong cùng thời gian; sức chứa phòng nhỏ hơn sĩ số lớp; giáo viên hoặc trợ giảng bị trùng lịch; thời gian bắt đầu hoặc kết thúc không hợp lệ; hệ thống từ chối lưu và hiển thị thông báo lỗi.

2.10.12. Quản lý ca học
Tên Use Case	Quản lý ca học
Tác nhân	Nhân viên học vụ, Giáo viên
Mục tiêu	Cho phép theo dõi và cập nhật thông tin từng ca học thực tế của lớp dựa trên lịch học đã được thiết lập.
Tiền điều kiện	Người dùng đã đăng nhập; lớp đã có lịch học; ca học thuộc một lớp đang hoạt động.
Hậu điều kiện	Thông tin ca học được ghi nhận hoặc cập nhật để phục vụ giảng dạy, điểm danh và theo dõi tiến độ học tập.
Luồng chính	1. Người dùng truy cập chức năng Quản lý ca học. 
2. Hệ thống hiển thị danh sách các ca học theo lớp và thời gian. 
3. Người dùng chọn một ca học cần xem hoặc cập nhật. 4. Hệ thống hiển thị ngày học, giờ học, phòng học, giáo viên, trợ giảng và trạng thái ca học. 
5. Giáo viên có thể ghi nhận nội dung đã giảng dạy của ca học. 
6. Nhân viên học vụ có thể cập nhật phòng học, giáo viên thay thế hoặc trạng thái ca học khi có thay đổi hợp lệ. 
7. Người dùng xác nhận thông tin cập nhật. 
8. Hệ thống kiểm tra tính hợp lệ của dữ liệu. 
9. Hệ thống lưu thông tin ca học và cập nhật trạng thái tương ứng.
Luồng thay thế	Ca học có thể được chuyển sang trạng thái Completed, Postponed hoặc Cancelled; nhân viên học vụ có thể thay đổi giáo viên hoặc phòng học khi phát sinh tình huống thực tế.
Luồng ngoại lệ	Ca học không tồn tại; lớp đã kết thúc; phòng hoặc giáo viên thay thế bị trùng lịch; người dùng không có quyền cập nhật trường thông tin tương ứng; hệ thống từ chối thao tác và hiển thị thông báo.

2.10.13. Xem lớp và lịch giảng dạy	
Tên Use Case	Xem lớp và lịch giảng dạy
Tác nhân	Giáo viên, Trợ giảng
Mục tiêu	Cho phép giáo viên và trợ giảng xem các lớp được phân công, danh sách học viên và lịch giảng dạy hoặc trợ giảng của bản thân.
Tiền điều kiện	Giáo viên hoặc trợ giảng đã đăng nhập và đã được phân công ít nhất một lớp.
Hậu điều kiện	Người dùng xem được thông tin các lớp và lịch được phân công mà không làm thay đổi dữ liệu.
Luồng chính	1. Giáo viên hoặc trợ giảng truy cập chức năng Lớp và lịch giảng dạy.
2. Hệ thống xác định các lớp được phân công cho tài khoản hiện tại. 
3. Hệ thống hiển thị danh sách lớp gồm mã lớp, tên lớp, khóa học, cấp độ và trạng thái. 
4. Người dùng chọn một lớp để xem chi tiết. 
5. Hệ thống hiển thị thông tin lớp và danh sách học viên. 6. Người dùng chọn chức năng xem lịch. 
7. Hệ thống hiển thị ngày học, giờ học, phòng học và các ca học tương ứng. 
8. Người dùng có thể lọc lịch theo ngày, tuần, tháng hoặc lớp.
Luồng thay thế	Nếu người dùng được phân công nhiều lớp, hệ thống cho phép lọc theo từng lớp hoặc khoảng thời gian; người dùng có thể chuyển giữa chế độ danh sách và lịch nếu hệ thống hỗ trợ.
Luồng ngoại lệ	Người dùng chưa được phân công lớp; phân công đã hết hiệu lực; lớp không còn tồn tại hoặc không còn quyền truy cập; hệ thống hiển thị thông báo phù hợp.

2.10.14. Điểm danh học viên
Tên Use Case	Điểm danh học viên
Tác nhân	Giáo viên, Trợ giảng
Mục tiêu	Cho phép giáo viên hoặc trợ giảng ghi nhận và cập nhật trạng thái tham gia của học viên trong từng ca học.
Tiền điều kiện	Người dùng đã đăng nhập; được phân công vào lớp; ca học đã tồn tại; học viên đã được xếp vào lớp.
Hậu điều kiện	Kết quả điểm danh của từng học viên trong ca học được lưu và có thể sử dụng để theo dõi chuyên cần.
Luồng chính	1. Giáo viên hoặc trợ giảng truy cập lớp được phân công. 
2. Người dùng chọn ca học cần điểm danh. 
3. Hệ thống hiển thị danh sách học viên thuộc lớp.
4. Người dùng chọn trạng thái điểm danh cho từng học viên như Present, Absent with Permission, Absent without Permission, Late hoặc Left Early. 
5. Người dùng nhập ghi chú nếu cần. 
6. Người dùng xác nhận lưu kết quả điểm danh. 
7. Hệ thống kiểm tra mỗi học viên chỉ có một kết quả điểm danh trong ca học. 
8. Hệ thống lưu thông tin điểm danh. 
9. Hệ thống thông báo lưu thành công và cập nhật danh sách.
Luồng thay thế	Người dùng có thể chỉnh sửa kết quả điểm danh đã ghi nhận nếu còn trong thời gian cho phép và có quyền cập nhật.
Luồng ngoại lệ	Người dùng không được phân công vào lớp; ca học không tồn tại; học viên không thuộc lớp; trạng thái điểm danh không hợp lệ; kết quả đã bị khóa không cho chỉnh sửa; hệ thống từ chối thao tác và hiển thị thông báo.

2.10.15. Quản lý điểm số
Tên Use Case	Quản lý điểm số
Tác nhân	Giáo viên
Mục tiêu	Cho phép giáo viên nhập, cập nhật, kiểm tra và xác nhận điểm số của học viên trong các lớp được phân công.
Tiền điều kiện	Giáo viên đã đăng nhập; được phân công giảng dạy lớp; lớp có học viên và đã có các loại điểm hoặc hình thức đánh giá tương ứng.
Hậu điều kiện	Điểm số và nhận xét của học viên được ghi nhận hoặc cập nhật và có thể được sử dụng cho việc đánh giá kết quả học tập.
Luồng chính	1. Giáo viên truy cập chức năng Quản lý điểm số. 
2. Hệ thống hiển thị các lớp giáo viên đang phụ trách. 
3. Giáo viên chọn lớp cần nhập điểm. 
4. Hệ thống hiển thị danh sách học viên và các loại điểm của khóa học. 
5. Giáo viên chọn loại điểm như Homework, Midterm hoặc Final.
6. Giáo viên nhập điểm cho từng học viên và có thể nhập nhận xét. 
7. Hệ thống kiểm tra giá trị điểm có nằm trong giới hạn cho phép hay không. 
8. Giáo viên lưu tạm hoặc xác nhận kết quả. 
9. Hệ thống lưu điểm và cập nhật trạng thái tương ứng. 
10. Hệ thống thông báo thao tác thành công.
Luồng thay thế	Giáo viên có thể sửa điểm trước khi xác nhận; có thể lưu tạm khi chưa hoàn tất toàn bộ danh sách; có thể cập nhật nhận xét mà không thay đổi điểm nếu được phép.
Luồng ngoại lệ	Giáo viên không được phân công lớp; học viên không thuộc lớp; loại điểm không áp dụng cho khóa học; điểm vượt quá giới hạn; điểm đã được khóa hoặc xác nhận và không còn quyền chỉnh sửa; hệ thống từ chối thao tác và hiển thị thông báo.

2.10.16. Xem kết quả và lịch học
Tên Use Case	Xem kết quả và lịch học
Tác nhân	Học viên
Mục tiêu	Cho phép học viên xem lịch học cá nhân, thông tin lớp, phòng học, giáo viên phụ trách và kết quả học tập của bản thân.
Tiền điều kiện	Học viên đã đăng nhập; đã được xếp vào ít nhất một lớp hoặc có dữ liệu học tập trong hệ thống.
Hậu điều kiện	Học viên xem được thông tin lịch học và kết quả học tập của bản thân mà không làm thay đổi dữ liệu.
Luồng chính	1. Học viên truy cập chức năng Lịch học và kết quả học tập. 2. Hệ thống xác định các lớp mà học viên đang hoặc đã tham gia. 
3. Hệ thống hiển thị lịch học gồm tên lớp, ngày học, giờ học, phòng học và giáo viên phụ trách. 
4. Học viên có thể lọc lịch theo ngày, tuần, tháng hoặc lớp. 5. Học viên chọn chức năng xem kết quả học tập. 
6. Hệ thống hiển thị các loại điểm như Homework, Midterm, Final hoặc các hình thức đánh giá khác. 
7. Hệ thống hiển thị điểm số, nhận xét của giáo viên và kết quả trung bình nếu có. 
8. Học viên có thể chọn từng lớp hoặc khóa học để xem lịch sử kết quả tương ứng.
Luồng thay thế	Nếu học viên đang tham gia nhiều lớp, hệ thống cho phép lựa chọn lớp cần xem; nếu chưa có điểm, hệ thống hiển thị trạng thái chưa có kết quả thay vì danh sách điểm.
Luồng ngoại lệ	Học viên chưa được xếp lớp; lớp hoặc lịch học không còn tồn tại; dữ liệu kết quả chưa được công bố; hệ thống hiển thị thông báo phù hợp.

2.10.17. Quản lý học phí và thanh toán
Tên Use Case	Quản lý học phí và thanh toán
Tác nhân	Nhân viên kế toán
Mục tiêu	Cho phép nhân viên kế toán theo dõi học phí của học viên, ghi nhận thanh toán, quản lý công nợ, hoàn tiền hoặc điều chỉnh khi cần thiết.
Tiền điều kiện	Nhân viên kế toán đã đăng nhập và có quyền quản lý học phí; học viên đã có đăng ký khóa học hoặc khoản học phí phát sinh.
Hậu điều kiện	Thông tin học phí, số tiền đã thanh toán, số tiền còn lại và trạng thái thanh toán của học viên được cập nhật chính xác.
Luồng chính	1. Nhân viên kế toán truy cập chức năng Quản lý học phí và thanh toán. 
2. Hệ thống hiển thị danh sách các khoản học phí của học viên. 
3. Nhân viên kế toán tìm kiếm học viên hoặc khoản học phí cần xử lý. 
4. Hệ thống hiển thị học phí gốc, khoản giảm giá, học bổng, tổng số tiền phải thanh toán, số tiền đã thanh toán và số tiền còn lại. 
5. Khi học viên thanh toán, nhân viên kế toán chọn chức năng Ghi nhận thanh toán. 
6. Nhân viên kế toán nhập số tiền, ngày thanh toán, phương thức thanh toán và mã giao dịch nếu có. 
7. Hệ thống kiểm tra tính hợp lệ của giao dịch. 
8. Hệ thống lưu Payment và cập nhật Paid Amount, Remaining Amount và Tuition Status. 
9. Nhân viên kế toán có thể lập biên nhận cho giao dịch. 10. Hệ thống cập nhật thông tin học phí và thông báo thao tác thành công.
Luồng thay thế	Nhân viên kế toán có thể ghi nhận thanh toán một phần; áp dụng giảm giá hoặc học bổng hợp lệ; thực hiện hoàn tiền hoặc điều chỉnh giao dịch; xem các khoản còn nợ theo từng học viên.
Luồng ngoại lệ	Số tiền thanh toán không hợp lệ; số tiền thanh toán vượt quá khoản còn phải trả mà không có điều chỉnh hợp lệ; học phí hoặc học viên không tồn tại; giao dịch bị trùng mã; hệ thống từ chối lưu và hiển thị thông báo lỗi.

2.10.18. Quản lý thông báo
Tên Use Case	Quản lý thông báo
Tác nhân	Nhân viên học vụ, Giáo viên
Mục tiêu	Cho phép người có quyền tạo và gửi thông báo đến học viên, giáo viên, trợ giảng hoặc các thành viên của lớp.
Tiền điều kiện	Người dùng đã đăng nhập và có quyền gửi thông báo; đối tượng nhận thông báo tồn tại trong hệ thống.
Hậu điều kiện	Thông báo được lưu và gửi đến đúng đối tượng nhận theo nội dung đã xác định.
Luồng chính	1. Người dùng truy cập chức năng Quản lý thông báo.
2. Hệ thống hiển thị danh sách các thông báo đã tạo. 
3. Người dùng chọn tạo thông báo mới. 
4. Hệ thống hiển thị biểu mẫu nhập thông báo. 
5. Người dùng nhập tiêu đề, nội dung và loại thông báo. 
6. Người dùng lựa chọn đối tượng nhận như một học viên, một lớp, giáo viên, trợ giảng hoặc nhóm người dùng phù hợp. 
7. Nếu thông báo liên quan đến lớp học, người dùng chọn lớp tương ứng. 
8. Người dùng xác nhận gửi thông báo. 
9. Hệ thống kiểm tra tính hợp lệ của nội dung và đối tượng nhận. 
10. Hệ thống lưu thông báo và gửi đến các đối tượng đã chọn. 
11. Hệ thống cập nhật trạng thái gửi và thông báo kết quả.
Luồng thay thế	Người dùng có thể lưu thông báo ở trạng thái nháp trước khi gửi; có thể chọn gửi cho toàn bộ thành viên của một lớp hoặc một nhóm người dùng thay vì từng người.
Luồng ngoại lệ	Nội dung thông báo bị thiếu; đối tượng nhận không tồn tại; người dùng không có quyền gửi đến đối tượng đã chọn; hệ thống từ chối thao tác và hiển thị thông báo.

2.10.19. Thống kê và báo cáo
Tên Use Case	Thống kê và báo cáo
Tác nhân	Quản trị viên hệ thống, Nhân viên học vụ, Nhân viên kế toán
Mục tiêu	Cho phép người có quyền tổng hợp, theo dõi và xuất các số liệu phục vụ quản lý hoạt động đào tạo và tài chính của trung tâm.
Tiền điều kiện	Người dùng đã đăng nhập và có quyền xem loại báo cáo tương ứng; hệ thống đã có dữ liệu cần thống kê.
Hậu điều kiện	Báo cáo hoặc số liệu thống kê được hiển thị và có thể được xuất ra định dạng phù hợp khi cần.
Luồng chính	1. Người dùng truy cập chức năng Thống kê và báo cáo. 
2. Hệ thống hiển thị các loại báo cáo mà vai trò hiện tại được phép sử dụng. 
3. Người dùng chọn loại báo cáo cần xem. 
4. Người dùng nhập khoảng thời gian hoặc các điều kiện lọc cần thiết. 
5. Hệ thống truy xuất và tổng hợp dữ liệu. 
6. Hệ thống hiển thị kết quả dưới dạng bảng số liệu. 
7. Với các báo cáo phù hợp, hệ thống hiển thị biểu đồ như bar chart hoặc pie chart. 
8. Người dùng có thể xuất báo cáo ra Excel hoặc PDF.
9. Hệ thống tạo file báo cáo và thông báo kết quả.
Luồng thay thế	Nhân viên học vụ có thể xem báo cáo số lượng học viên, lớp học, tỷ lệ điểm danh và kết quả học tập; nhân viên kế toán có thể xem báo cáo học phí, thanh toán, công nợ và doanh thu; quản trị viên có thể xem các báo cáo tổng hợp trong phạm vi được phân quyền.
Luồng ngoại lệ	Không có dữ liệu phù hợp với điều kiện lọc; khoảng thời gian không hợp lệ; người dùng không có quyền xem loại báo cáo đã chọn; quá trình xuất file gặp lỗi; hệ thống hiển thị thông báo phù hợp.

2.10.20. Sao lưu và phục hồi dữ liệu
Tên Use Case	Sao lưu và phục hồi dữ liệu
Tác nhân	Quản trị viên hệ thống
Mục tiêu	Cho phép quản trị viên tạo bản sao lưu dữ liệu và phục hồi hệ thống từ bản sao lưu khi xảy ra sự cố hoặc có nhu cầu khôi phục dữ liệu.
Tiền điều kiện	Quản trị viên đã đăng nhập và có quyền sao lưu, phục hồi dữ liệu; hệ thống có quyền truy cập đến vị trí lưu trữ bản sao lưu.
Hậu điều kiện	Dữ liệu được sao lưu thành công hoặc hệ thống được phục hồi về trạng thái của bản sao lưu được lựa chọn.
Luồng chính	1. Quản trị viên truy cập chức năng Sao lưu và phục hồi dữ liệu. 
2. Hệ thống hiển thị danh sách các bản sao lưu hiện có và các chức năng tương ứng. 
3. Quản trị viên chọn chức năng Sao lưu dữ liệu. 
4. Hệ thống xác nhận yêu cầu sao lưu.
5. Quản trị viên xác nhận thực hiện. 
6. Hệ thống tạo bản sao lưu dữ liệu và lưu thông tin thời gian tạo, người thực hiện và vị trí lưu trữ. 
7. Hệ thống thông báo sao lưu thành công. 
8. Khi cần phục hồi, quản trị viên chọn một bản sao lưu hợp lệ. 
9. Hệ thống hiển thị thông tin bản sao lưu và cảnh báo việc phục hồi có thể thay đổi dữ liệu hiện tại. 
10. Quản trị viên xác nhận phục hồi. 
11. Hệ thống thực hiện phục hồi dữ liệu từ bản sao lưu. 
12. Hệ thống kiểm tra kết quả và thông báo phục hồi thành công.
Luồng thay thế	Quản trị viên có thể hủy thao tác trước khi quá trình sao lưu hoặc phục hồi bắt đầu; có thể lựa chọn bản sao lưu khác nếu chưa xác nhận phục hồi.
Luồng ngoại lệ	Không đủ dung lượng lưu trữ; bản sao lưu bị lỗi hoặc không hợp lệ; mất kết nối cơ sở dữ liệu; quá trình phục hồi thất bại; hệ thống dừng thao tác, ghi nhận lỗi và thông báo cho quản trị viên.

2.11. SƠ ĐỒ LỚP PHÂN TÍCH 
2.11.1. Xác định các lớp phân tích
2.11.2. Quan hệ giữa các lớp
2.11.3. Sơ đồ lớp phân tích 
 
Nên vẽ lại giống vậy  
2.12. KẾT CHƯƠNG

CHƯƠNG 3: THIẾT KẾ HỆ THỐNG
3.1. Kiến trúc tổng thể hệ thống
3.2. Thiết kế phân quyền người dùng
3.3. Sơ đồ lớp thiết kế
3.3.1. Xác định các lớp thiết kế
3.3.2. Quan hệ giữa các lớp
3.3.3. Sơ đồ lớp thiết kế tổng thể
3.4. Thiết kế cơ sở dữ liệu
3.4.1. Xác định các thực thể
3.4.2. Sơ đồ ERD
3.4.3. Mô hình dữ liệu quan hệ
3.4.4. Mô tả các bảng dữ liệu
3.5. Các ràng buộc dữ liệu
3.6. Thiết kế giao diện
3.6.1. Giao diện WinForms dành cho Quản trị viên
3.6.2. Giao diện Website dành cho Nhân viên học vụ, Nhân viên tư vấn, Giáo viên, Trợ giảng và Nhân viên kế toán
3.6.3. Giao diện Website dành cho Học viên
CHƯƠNG 4: CÀI ĐẶT HỆ THỐNG
Nội dung trình bày liên quan đến việc cài đặt/cấu hình các thành phần hệ thống/thuật toán liên quan; Trình bày rõ môi trường cài đặt và đặc tả tương ứng.
CHƯƠNG 5: THỬ NGHIỆM VÀ TRIỂN KHAI
Trình bày các nội dung liên quan đến việc thử nghiệm, triển khai hệ thống / đánh giá mô hình, thuật toán.

KẾT LUẬN
–	Kết quả đạt được: Trình bày tóm tắt kết quả đạt được của đề tài
–	Đánh giá kết quả: Xác định hệ thống đã hoàn thành được những yêu cầu nào, mức độ hoàn thiện bao nhiêu so với mục tiêu ban đầu.
–	Hạn chế: Nêu ra những điểm còn hạn chế, khó khăn gặp phải trong quá trình thực hiện.
–	Hướng phát triển: Đề xuất các cải tiến hoặc mở rộng để phát triển hệ thống hoàn thiện hơn trong tương lai.

TÀI LIỆU THAM KHẢO

[1] Trần Minh Hải, Giáo trình Phát triển hệ thống, NXB Giáo Dục, 2024
[2] Peter MacIntyre, Kevin Tatroe, Programming PHP 4th Edition, O'Reilly Media, Inc, 2020
[3] Unil Yun, Donggyu Kim, Mining of high average-utility itemsets using novel list structure and pruning strategy, Future Generation Computer Systems, vol. 68, pp. 346-360, 2017
[4] https://react.dev/learn, truy cập ngày 12/03/2025 










QUI ĐỊNH THỂ THỨC TRÌNH BÀY
1. Yêu cầu chung
–	Tất cả nội dung đều sử dụng font Times New Roman.
–	Lề trang: Trái 3.5 cm, phải 2.5 cm, trên 2.5 cm, dưới 2.5 cm.
–	Tất cả nội dung in trên một mặt giấy A4, Trang bìa chính in trên giấy bìa, Trang bìa phụ in giấy trắng thường.
2. Định dạng đề mục số phân cấp, đề mục hình ảnh, bảng biểu
–	Cấp chương: cỡ chữ 18, in hoa, canh giữa, tô đậm.
–	Đề mục số cấp 1.1: cỡ chữ 14, in hoa, tô đậm.
–	Đề mục số cấp 1.1.1. và cấp 1.1.1.1 (nếu có): Cỡ chữ 13, in thường, tô đậm.
–	Đề mục hình: Đánh chỉ mục theo chương và đặt phía dưới hình, canh giữa. Trong chương 1 bắt đầu bằng: Hình 1.1, Hình 1.2,..Chương 2 bắt đầu bằng Hình 2.1, Hình 2.2,...
Ví dụ: 
 
Hình 1.1: Giao diện trang chủ
–	Đề mục bảng: Tương tự như đề mục hình nhưng đặt phía trên bảng, canh giữa. Nội dung trong bảng có cỡ chữ 12, Line spacing Multiple at 1.3, Before 0pt, After 0pt. Lặp lại tiêu đề bảng nếu một phần bảng sang trang mới.
Bảng 1.1: Dữ liệu thực nghiệm
STT	Loại dữ liệu	Ghi chú
1		
2		
3		
3. Định dạng nội dung, bullet
–	Nội dung: Viết dạng đoạn văn, thục vào đầu dòng mỗi đoạn. Cỡ chữ 13, Line spacing Multiple at 1.3, Before 6pt, After 6pt, canh đều 2 bên. Mô tả, diễn giải vấn đề rõ ràng, mạch lạc theo văn phong khoa học. 
–	Bullet cấp 1 sử dụng dấu gạch “”; Line spacing Multiple at 1.3, Before 6pt, After 6pt, canh đều 2 bên; Position 0.3 cm, Text indent 0.8 cm.
+	Bullet cấp 2 sử dụng dấu “+”; Position 0.8 cm, Text indent 1.3 cm. Định dạng paragraph giống như Bullet cấp 1.
2.12.1. CÁCH CHỈNH Bullet cấp 1 của đoạn bạn gửi
1.	Bôi đen 6 dòng bắt đầu từ: 
o	Ứng dụng WinForms 
o	Backend API 
o	Website 
o	Giao diện Website 
o	Cơ sở dữ liệu 
o	Kết nối dữ liệu 
2.	Vào Home → Bullets ▼ → Define New Bullet → Symbol
→ chọn dấu -. 
3.	Vẫn bôi đen 6 dòng → chuột phải → Adjust List Indents... 
Điền:
•	Bullet position: 0.3 cm 
•	Text indent: 0.8 cm 
→ OK
4.	Vẫn bôi đen → chuột phải → Paragraph... 
Điền:
•	Alignment: Justified 
•	Before: 6 pt 
•	After: 6 pt 
•	Line spacing: Multiple 
•	At: 1.3 
→ OK

4. Số trang, mục lục
–	Số trang đánh phía dưới mỗi trang, canh giữa. Không đánh số trang bìa chính, bìa phụ. Từ trang Lời cảm ơn cho đến Danh mục bảng biểu đánh số trang dạng la mã i, ii, iii,...Từ trang Mở đầu trở đi đánh số 1,2,3,...
–	Mục lục: Tạo mục lục tự động gồm các đề mục như ví dụ trình bày ở trang phần mục lục.







