// GUI\frmEditMajor.cs
using System;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Lab05.GUI
{
    public partial class frmEditMajor : Form
    {
        private readonly FacultyService facultyService = new FacultyService();
        private readonly MajorService majorService = new MajorService();

        public frmEditMajor()
        {
            InitializeComponent();
        }

        private void frmEditMajor_Load(object sender, EventArgs e)
        {
            var faculties = facultyService.GetAll();
            cmbFaculty.DataSource = faculties;
            cmbFaculty.DisplayMember = "FacultyName";
            cmbFaculty.ValueMember = "FacultyID";

            LoadMajors();
        }

        private void LoadMajors()
        {
            int facultyID;
            if (!int.TryParse(cmbFaculty.SelectedValue?.ToString(), out facultyID))
            {
                cmbMajor.DataSource = null;
                ClearFields();
                return;
            }

            var majors = majorService.GetMajorsByFaculty(facultyID);
            cmbMajor.DataSource = majors;
            cmbMajor.DisplayMember = "Name";
            cmbMajor.ValueMember = "MajorID";

            if (majors.Count > 0)
                ShowMajorInfo();
            else
                ClearFields();
        }


        private void ShowMajorInfo()
        {
            int facultyID;
            int majorID;
            if (!int.TryParse(cmbFaculty.SelectedValue?.ToString(), out facultyID) ||
                !int.TryParse(cmbMajor.SelectedValue?.ToString(), out majorID))
            {
                ClearFields();
                return;
            }

            var major = majorService.FindById(facultyID, majorID);
            if (major != null)
            {
                txtMajorID.Text = major.MajorID.ToString();
                txtName.Text = major.Name;
            }
            else
            {
                ClearFields();
            }
        }

        private void ClearFields()
        {
            txtMajorID.Clear();
            txtName.Clear();
        }

        private void cmbFaculty_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMajors();
        }

        private void cmbMajor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMajor.SelectedValue != null)
                ShowMajorInfo();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên chuyên ngành!");
                    return;
                }

                int facultyID, majorID;
                if (!int.TryParse(cmbFaculty.SelectedValue?.ToString(), out facultyID) ||
                    !int.TryParse(cmbMajor.SelectedValue?.ToString(), out majorID))
                {
                    MessageBox.Show("Vui lòng chọn đầy đủ Khoa và Chuyên ngành!");
                    return;
                }

                string name = txtName.Text.Trim();
                majorService.Update(facultyID, majorID, name);
                MessageBox.Show("Cập nhật thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadMajors(); // Tải lại để cập nhật
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}