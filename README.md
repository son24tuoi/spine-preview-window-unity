Created: May 24, 2026 12:52 PM

# Spine Preview Window - Thêm/Xóa Event

Spine Preview Window là một bảng điều khiển trực quan trong Unity Engine, cho phép các nhà phát triển game và chuyên viên animator kiểm tra, tinh chỉnh và quản lý nhanh các tệp Spine 2D mà không cần phải mở phần mềm Spine chuyên dụng hay chạy trực tiếp trò chơi.

<aside>
💡

Có thể thêm/xóa sự kiện (events) trong animation một cách nhanh chóng, thay vì phải phụ thuộc vào phần mềm Spine như thường lệ.

</aside>

Công cụ hỗ trợ trong quá trình làm việc, giúp phát hiện nhanh các lỗi hiển thị, đồng bộ hóa sự kiện âm thanh/ hiệu ứng (events) cực kỳ hiệu quả.

> **Cách mở Spine Preview**
Trên thanh menu công cụ, chọn mục Tools → Spine Preview.
> 

![SpinePreview-Demo.png](/Assets/SpinePreviewWindow/Images/SpinePreview-Demo.png)

# Lợi ích khi sử dụng

- **Tiết kiệm thời gian:** Kiểm tra nhanh chuyển động của nhân vật ngay trong Editor mà không cần tốn thời gian play.
- **Quản lý Event trực quan:** Gắn các sự kiện âm thanh/ hiệu ứng chính xác (mili giây) dựa trên Timeline trực quan.
- **Kiểm tra lỗi hiển thị nhanh chóng:** Dễ dàng xem xương hoặc lưới để phát hiện các lỗi rách ảnh, lỗi biến dạng khi nhân vật chuyển động.

# Các thành phần giao diện chính

## 1. Khu vực hiển thị trực quan

- **Preview Area:** Chiếm không gian lớn bên trái, hiển thị trực quan đối tượng và các chuyển động hoạt họa. Người dùng có thể quan sát trực tiếp nhân vật.
    - Lăn chuột để thu phóng đối tượng.
    - Kéo chuột trái để di chuyển đối tượng.
- **Timeline Area:** Nằm phía dưới cùng bên trái, hiển thị tiến trình của hoạt họa với vạch thời gian (màu đỏ) giúp người dùng biết chính xác hoạt họa đang chạy đến đâu hoặc các vị trí sự kiện được kích hoạt (màu xanh).
    - Click hoặc kéo chuột trái để thay đổi thời điểm hiển thị hoạt họa.

## 2. Khu vực điều khiển

- **Skeleton Data Asset:** Chọn file Spine.
- **Spine Json Asset:** Tự động load dựa theo Skeleton Data Asset.
    - Chỉ sửa được Event nếu file này là dạng JSON.
- **Show Triangle Mesh:** Hiển thị lưới tam giác của đối tượng.
- **Show Bones:** Hiển thị hệ thống xương của đối tượng.
- **Fit Camera:** Căn chỉnh lại camera vừa với đối tượng.
- **Skin:** Chọn skin của đối tượng.
- **Setup Pose:** Đưa đối tượng về tư thế mặc định.
- **Animations**: Chọn animation muốn chạy thử.
- **Loop:** Animation chạy tự lặp lại.
- **Current Time:** Hiển thị thời gian chạy hiện tại, có thể thay đổi.
- **Speed:** Thanh trượt điều chỉnh tốc độ phát animation (nhanh/chậm).
- **Play:** Chạy animation.
- **Pause:** Tạm dừng animation.
- **Stop:** Dừng chạy animation.
- **Event List:** Hiển thị danh sách các sự kiện có trong animation.
    - Nhấn vào sự kiện để nhảy tới mốc thời gian tương ứng.
    - **Delete:** xóa sự kiện.
    - **Create Event:** Tạo sự kiện mới.
        - **Name:** Nhập tên sự kiện mới.
        - **Time:** Nhập mốc thời gian sự kiện mới.
        - **Add Event:** Tạo sự kiện mới theo nội dung đã nhập.
- **Save Events:** Lưu lại các thay đổi sự kiện nếu cần.
    
    <aside>
    💡
    
    **Lưu ý:** Khi Save sẽ tạo ra một file data spine JSON mới và có file Skeleton Data Asset mới chứa nội dung (không dùng file vừa sửa).
    
    Ví dụ file gốc là `spineboy-unity.json` thì file mới sẽ là `spineboy-unity_202605241206.json`. Phần tên thêm mới là thời gian tạo file.
    
    Nếu muốn xem những thay đổi đã lưu thì chọn lại file Skeleton Data Asset.
    
    </aside>