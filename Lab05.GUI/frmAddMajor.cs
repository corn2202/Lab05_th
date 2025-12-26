using System;
using System.Windows.Forms;
using Lab05.DAL;

namespace Lab05.GUI
{
    public partial class frmAddMajor : Form
    {
        private readonly MajorService _majorService;
        private readonly FacultyService _facultyService;

        public frmAddMajor()
        {
            InitializeComponent();
            _majorService = new MajorService();
            _facultyService = new FacultyService(); // Assuming you have this
            LoadFaculties();
        }

        private void LoadFaculties()
        {
            var faculties = _facultyService.GetAll();
            cmbFaculty.DataSource = faculties;
            cmbFaculty.DisplayMember = "FacultyName";
            cmbFaculty.ValueMember = "FacultyID";
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(txtMajorName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên chuyên ngành!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbFaculty.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn khoa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string majorName = txtMajorName.Text.Trim();
            int facultyId = (int)cmbFaculty.SelectedValue;

            try
            {
                // Gọi service để thêm
                _majorService.Add(majorName, facultyId);
                MessageBox.Show("Thêm chuyên ngành thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK; // Thông báo form cha có thể reload danh sách
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm chuyên ngành: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}