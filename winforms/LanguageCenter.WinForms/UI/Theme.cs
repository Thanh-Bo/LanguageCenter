namespace LanguageCenter.WinForms.UI;

/// <summary>
/// Bảng màu và phong cách giao diện 
public static class Theme
{
    // Màu thương hiệu 
    public static readonly Color Primary = Color.FromArgb(30, 86, 240); // #1E56F0
    public static readonly Color PrimaryDark = Color.FromArgb(23, 67, 197); // #1743C5
    public static readonly Color PrimaryLight = Color.FromArgb(238, 244, 255); // #EEF4FF
    public static readonly Color PrimaryBorder = Color.FromArgb(191, 219, 254); // #BFDBFE

    // Sidebar & Navigation (Deep Slate Navy)
    public static readonly Color SidebarBackground = Color.FromArgb(15, 23, 42); // #0F172A
    public static readonly Color SidebarButtonHover = Color.FromArgb(30, 41, 59); // #1E293B
    public static readonly Color SidebarButtonActive = Color.FromArgb(30, 86, 240); // #1E56F0
    public static readonly Color SidebarText = Color.FromArgb(241, 245, 249);
    public static readonly Color SidebarTextMuted = Color.FromArgb(148, 163, 184);

    // Nền & Khối nội dung
    public static readonly Color HeaderBackground = Color.White;
    public static readonly Color ContentBackground = Color.FromArgb(248, 250, 252); // #F8FAFC
    public static readonly Color CardBackground = Color.White;

    // Màu trạng thái & ngữ cảnh
    public static readonly Color Danger = Color.FromArgb(239, 68, 68); // #EF4444
    public static readonly Color DangerDark = Color.FromArgb(220, 38, 38);
    public static readonly Color DangerLight = Color.FromArgb(254, 242, 242);
    public static readonly Color Success = Color.FromArgb(16, 185, 129); // #10B981 Emerald
    public static readonly Color SuccessLight = Color.FromArgb(236, 253, 245);
    public static readonly Color Warning = Color.FromArgb(245, 158, 11); // #F59E0B Amber
    public static readonly Color WarningLight = Color.FromArgb(254, 243, 199);
    public static readonly Color Info = Color.FromArgb(14, 165, 233); // #0EA5E9 Sky
    public static readonly Color Purple = Color.FromArgb(139, 92, 246); // #8B5CF6

    // Màu chữ & viền
    public static readonly Color TextDark = Color.FromArgb(15, 23, 42); // #0F172A
    public static readonly Color TextMedium = Color.FromArgb(71, 85, 105); // #475569
    public static readonly Color Muted = Color.FromArgb(100, 116, 139); // #64748B
    public static readonly Color BorderColor = Color.FromArgb(226, 232, 240); // #E2E8F0

    // Phông chữ hệ thống
    public static readonly Font FontRegular = new("Segoe UI", 9.5f);
    public static readonly Font FontBold = new("Segoe UI", 9.5f, FontStyle.Bold);
    public static readonly Font FontTitle = new("Segoe UI", 15f, FontStyle.Bold);
    public static readonly Font FontHeading = new("Segoe UI", 11.5f, FontStyle.Bold);
    public static readonly Font FontStat = new("Segoe UI", 22f, FontStyle.Bold);
    public static readonly Font FontBrand = new("Segoe UI", 18f, FontStyle.Bold);
    public static readonly Font FontSub = new("Segoe UI", 8.5f);
}

/// Các hàm dựng/định dạng control lặp lại nhiều lần trong các màn hình
public static class UiHelpers
{
    public static Button CreatePrimaryButton(string text, int width = 140, int height = 36)
    {
        var button = new Button
        {
            Text = text,
            Width = width,
            Height = height,
            FlatStyle = FlatStyle.Flat,
            BackColor = Theme.Primary,
            ForeColor = Color.White,
            Font = Theme.FontBold,
            Cursor = Cursors.Hand
        };
        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = Theme.PrimaryDark;
        return button;
    }

    public static Button CreateSecondaryButton(string text, int width = 120, int height = 36)
    {
        var button = new Button
        {
            Text = text,
            Width = width,
            Height = height,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.White,
            ForeColor = Theme.TextDark,
            Font = Theme.FontRegular,
            Cursor = Cursors.Hand
        };
        button.FlatAppearance.BorderSize = 1;
        button.FlatAppearance.BorderColor = Theme.BorderColor;
        button.FlatAppearance.MouseOverBackColor = Theme.PrimaryLight;
        return button;
    }

    public static Button CreateDangerButton(string text, int width = 130, int height = 36)
    {
        var button = new Button
        {
            Text = text,
            Width = width,
            Height = height,
            FlatStyle = FlatStyle.Flat,
            BackColor = Theme.Danger,
            ForeColor = Color.White,
            Font = Theme.FontBold,
            Cursor = Cursors.Hand
        };
        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = Theme.DangerDark;
        return button;
    }

    public static void StyleGrid(DataGridView grid)
    {
        grid.BorderStyle = BorderStyle.None;
        grid.BackgroundColor = Color.White;
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        grid.GridColor = Theme.BorderColor;
        grid.RowHeadersVisible = false;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.AllowUserToResizeRows = false;
        grid.ReadOnly = true;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.MultiSelect = false;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.Font = Theme.FontRegular;
        grid.RowTemplate.Height = 36;
        grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        grid.ColumnHeadersHeight = 40;
        grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = Theme.ContentBackground,
            ForeColor = Theme.TextDark,
            Font = Theme.FontBold,
            Alignment = DataGridViewContentAlignment.MiddleLeft,
            Padding = new Padding(10, 0, 0, 0)
        };
        grid.DefaultCellStyle = new DataGridViewCellStyle
        {
            SelectionBackColor = Color.FromArgb(238, 244, 255),
            SelectionForeColor = Theme.PrimaryDark,
            ForeColor = Theme.TextDark,
            Padding = new Padding(10, 0, 0, 0)
        };
        grid.EnableHeadersVisualStyles = false;
    }

    public static TextBox CreateTextBox(int width = 200)
        => new() { Width = width, Font = Theme.FontRegular };

    public static ComboBox CreateComboBox(int width = 160)
        => new() { Width = width, Font = Theme.FontRegular, DropDownStyle = ComboBoxStyle.DropDownList };

    public static Label CreateLabel(string text, bool bold = false)
        => new() { Text = text, AutoSize = true, Font = bold ? Theme.FontBold : Theme.FontRegular, ForeColor = Theme.TextDark };

    public static Panel CreateCard(int width, int height)
        => new()
        {
            Width = width,
            Height = height,
            BackColor = Color.White,
            Padding = new Padding(18),
            Margin = new Padding(0, 0, 16, 16)
        };

    public static void PaintCardBorder(object? sender, PaintEventArgs e)
    {
        if (sender is Control control)
        {
            using var pen = new Pen(Theme.BorderColor, 1);
            e.Graphics.DrawRectangle(pen, 0, 0, control.Width - 1, control.Height - 1);
        }
    }

    public static string FormatDate(DateOnly? date) => date is null ? "—" : date.Value.ToString("dd/MM/yyyy");

    public static string FormatDate(DateOnly date) => date.ToString("dd/MM/yyyy");

    public static string FormatDateTime(DateTime? dateTime) => dateTime is null ? "—" : dateTime.Value.ToLocalTime().ToString("dd/MM/yyyy HH:mm");

    public static string FormatDateTime(DateTime dateTime) => dateTime.ToLocalTime().ToString("dd/MM/yyyy HH:mm");

    public static Color StatusColor(string status) => status switch
    {
        "Hoạt động" or "Đang học" or "Đang làm việc" or "Đang mở" => Theme.Success,
        "Bị khóa" or "Bảo lưu" or "Sắp mở" => Theme.Warning,
        "Ngừng sử dụng" or "Đã nghỉ" or "Đã nghỉ việc" or "Đã kết thúc" => Theme.Danger,
        _ => Theme.Muted
    };
}
