namespace RanseiLink.Windows.Controls
{
    public static class IconUtil
    {
        public static string IconToStr(IconId icon)
        {
            int num = (int)icon;
            char chr = (char)num;
            return new string(chr, 1);
        }
    }

    // Get latest font from https://github.com/google/material-design-icons/blob/master/variablefont/MaterialSymbolsOutlined%5BFILL%2CGRAD%2Copsz%2Cwght%5D.ttf
    // Find an icon's code by name https://fonts.google.com/icons?icon.size=24&icon.color=%23e8eaed
    public enum IconId
    {
        search = 0xe8b6,
        input = 0xe890,
        undo = 0xe166,
        redo = 0xe15a,
        check_circle = 0xe86c,
        add = 0xe145,
        check = 0xe5ca,
        output = 0xebbe,
        _3d_rotation = 0xe84d,
        visibility = 0xe8f4,
        publish = 0xe255,
        category = 0xe574,
        resize = 0xf707,
        delete = 0xe872,
        image = 0xe3f4,
        edit_note = 0xe745,
        content_copy = 0xe14d,
        content_cut = 0xe14e,
        content_paste = 0xe14f,
        bug_report = 0xe868,
        note_add = 0xe89c,
        backspace = 0xe14a,
        sell = 0xf05b,
        library_add = 0xe02e,
        extension = 0xe87b,
        folder_open = 0xe2c8,
        upload_file = 0xe9fc,
        drive_folder_upload = 0xe9a3,
        download = 0xf090,
        arrow_back = 0xe5c4,
        arrow_forward = 0xe5c8,
        sort = 0xe164,
        bolt = 0xea0b,
        chevron_left = 0xe5cb,
        chevron_right = 0xe5cc,
        chevron_up = 0xe5ce,
        chevron_down = 0xe5cf,
        rotate_right = 0xe41a, 
        rotate_left = 0xe419,
        keep = 0xe6aa,
        fiber_new = 0xe05e,
        circle = 0xef4a,

    }
}
