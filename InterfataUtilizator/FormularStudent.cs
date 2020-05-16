using System;
using System.Drawing;
using System.Windows.Forms;
using LibrarieModele;
using NivelAccesDate;
using System.Collections.Generic;

namespace InterfataUtilizator
{
    public partial class FormularStudent : Form
    {
        IStocareData adminStudenti;
        List<string> disciplineSelectate = new List<string>();

        public FormularStudent()
        {
            InitializeComponent();
            adminStudenti = StocareFactory.GetAdministratorStocare(); 
        }
        
        private void btnAdauga_Click(object sender, EventArgs e)
        {
            ResetCuloareEtichete();

            CodEroare codValidare = Validare(txtNume.Text, txtPrenume.Text, txtNote.Text);

            if (codValidare != CodEroare.CORECT)
            {
                MarcheazaControaleCuDateIncorecte(codValidare);
            }
            else
            {
                Student s = new Student(txtNume.Text, txtPrenume.Text);
                s.SetNote(txtNote.Text);

                //set program studiu
                //verificare radioButton selectat
                s.ProgramSTD = GetProgramStudiuSelectat();
                //set Discipline
                s.Discipline = new List<string>();
                s.Discipline.AddRange(disciplineSelectate);
                s.AnStudiu = Int32.Parse(cmbAnStudiu.Text);

                adminStudenti.AddStudent(s);
                lblMesaj.Text = "Studentul a fost adaugat";

                //resetarea controalelor pentru a introduce datele unui student nou
                ResetareControale();
            }
        }

        private void SetCkbValues(string[] values)
        {
            ckbDisciplina1.Text = values[0];
            ckbDisciplina1.Visible = true;
            ckbDisciplina2.Text = values[1];
            ckbDisciplina2.Visible = true;
            ckbDisciplina3.Text = values[2];
            ckbDisciplina3.Visible = true;
            ckbDisciplina4.Text = values[3];
            ckbDisciplina4.Visible = true;
            ckbDisciplina5.Text = values[4];
            ckbDisciplina5.Visible = true;
            ckbDisciplina6.Text = values[5];
            ckbDisciplina6.Visible = true;
        }

        private void SetDiscipline(object sender, EventArgs e)
        {
            ComboBox cmbAnStudiu = sender as ComboBox;
            string an = cmbAnStudiu.Text;
            switch(an)
            {
                case "1":
                    string[] an1 = new string[6] { "PCLP1", "PCLP2", "GAC", "ALGAD", "ASC", "FIZICA1" };
                    SetCkbValues(an1);
                    break;
                case "2":
                    string[] an2 = new string[6] { "POO", "PCLP3", "DEEA1", "DEEA2", "MEST", "FIZICA2" };
                    SetCkbValues(an2);
                    break;
                case "3":
                    string[] an3 = new string[6] { "BD", "PA", "MEA1", "EAEE", "IA", "PS" };
                    SetCkbValues(an3);
                    break;
                case "4":
                    string[] an4 = new string[6] { "RF", "CSI", "SIM", "PVLSI", "MEST", "PDS" };
                    SetCkbValues(an4);
                    break;
            }
        }

        private void btnAfiseaza_Click(object sender, EventArgs e)
        {
            lstAfisare.Items.Clear();
            var antetTabel = String.Format("{0,-5}{1,-35}{2,20}{3,10}\n", "Id", "Nume Prenume", "ProgramStudiu", "Medie");
            lstAfisare.Items.Add(antetTabel);

            List<Student> studenti = adminStudenti.GetStudenti();
            foreach(Student s in studenti)
            {
                var linieTabel = String.Format("{0,-5}{1,-35}{2,20}{3,10}\n", s.IdStudent, s.NumeComplet, s.ProgramSTD.ToString(), s.Media.ToString());
                lstAfisare.Items.Add(linieTabel);
            }
        }

        private void btnCauta_Click(object sender, EventArgs e)
        {
            Student s = adminStudenti.GetStudent(txtNume.Text, txtPrenume.Text);
            if (s != null)
            {
                lblMesaj.Text = s.ConversieLaSir();
                foreach (var disciplina in gpbDiscipline.Controls)
                {
                    if (disciplina is CheckBox)
                    {
                        var disciplinaBox = disciplina as CheckBox;
                        foreach (String dis in s.Discipline)
                            if (disciplinaBox.Text.Equals(dis))
                                disciplinaBox.Checked = true;
                    }
                }
            }
            else
                lblMesaj.Text = "Nu s-a gasit studentul";
            if (txtNume.Enabled == true && txtPrenume.Enabled==true)
            {
                txtNume.Enabled = false;
                txtPrenume.Enabled = false;
                //dezactivare butoane radio
                foreach (var button in gpbProgrameStudiu.Controls)
                {
                    if (button is RadioButton)
                    {
                        var radioButton = button as RadioButton;
                        radioButton.Enabled = false;
                    }
                }
            }
            else
            {
                txtNume.Enabled = true;
                txtPrenume.Enabled = true;
                //activare butoane radio
                foreach (var button in gpbProgrameStudiu.Controls)
                {
                    if (button is RadioButton)
                    {
                        var radioButton = button as RadioButton;
                        radioButton.Enabled = true;
                    }
                }
            }
        }

        private void btnModifica_Click(object sender, EventArgs e)
        {
            ResetCuloareEtichete();

            CodEroare codValidare = Validare(txtNume.Text, txtPrenume.Text, txtNote.Text);

            if (codValidare != CodEroare.CORECT)
            {
                MarcheazaControaleCuDateIncorecte(codValidare);
            }
            else
            {
                Student s = new Student(txtNume.Text, txtPrenume.Text);
                s.IdStudent = Int32.Parse(lblID.Text);
                s.SetNote(txtNote.Text);

                //set program studiu
                //verificare radioButton selectat
                s.ProgramSTD = GetProgramStudiuSelectat();
                //set Discipline
                s.Discipline = new List<string>();
                s.Discipline.AddRange(disciplineSelectate);
                s.AnStudiu = Int32.Parse(cmbAnStudiu.Text);

                adminStudenti.UpdateStudent(s);
                lblMesaj.Text = "Studentul a fost actualizat";

                //resetarea controalelor pentru a introduce datele unui student nou
                ResetareControale();
            }
        }

        private void ckbDiscipline_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBoxControl = sender as CheckBox; //operator 'as'
            //sau
            //CheckBox checkBoxControl = (CheckBox)sender;  //operator cast

            string disciplinaSelectata = checkBoxControl.Text;

            //verificare daca checkbox-ul asupra caruia s-a actionat este selectat
            if (checkBoxControl.Checked == true)
                disciplineSelectate.Add(disciplinaSelectata);
            else
                disciplineSelectate.Remove(disciplinaSelectata);
        }

        private void ResetareControale()
        {
            txtNume.Text = txtPrenume.Text = txtNote.Text = string.Empty;
            rdbCalculatoare.Checked = false;
            rdbAutomatica.Checked = false;
            rdbElectronica.Checked = false;
            rdbElectrotehnica.Checked = false;
            rdbInginerieE.Checked = false;
            ckbDisciplina1.Checked = false;
            ckbDisciplina2.Checked = false;
            ckbDisciplina3.Checked = false;
            ckbDisciplina4.Checked = false;
            ckbDisciplina5.Checked = false;
            ckbDisciplina6.Checked = false;
            disciplineSelectate.Clear();
            cmbAnStudiu.Text = string.Empty;
            lblMesaj.Text = string.Empty;

            lblID.Text = String.Empty;
        }

        private CodEroare Validare(string nume, string prenume, string sirNote)
        {
            CodEroare rezultatValidare = CodEroare.CORECT;
            if (nume == string.Empty)
            {
                rezultatValidare |= CodEroare.NUME_INCORECT;
            }
            if (prenume == string.Empty)
            {
                rezultatValidare |= CodEroare.PRENUME_INCORECT;
            }
            if (sirNote == string.Empty)
            {
                rezultatValidare |= CodEroare.NOTE_INCORECTE;
            }
            // verificare ca este cel putin un program studiu selectat
            int programStudiuSelectat = 0;
            foreach (var control in gpbProgrameStudiu.Controls)
            {
                RadioButton rdb = null;
                try
                {
                    rdb = (RadioButton)control;
                }
                catch { }

                if (rdb != null && rdb.Checked == true)
                    programStudiuSelectat = 1;
            }
            if (programStudiuSelectat == 0)
                rezultatValidare |= CodEroare.NO_PROGRAM_STUDIU;

            // verificarea numarului de note valide sa fie egal cu numarul de discipline selectate
            int[] note = Note.ExtrageNoteDinSir(sirNote);
            if (disciplineSelectate.Count != note.Length)
                rezultatValidare |= CodEroare.DISCIPLINE_NOTE;
            return rezultatValidare;
        }

        private void MarcheazaControaleCuDateIncorecte(CodEroare codValidare)
        {
            if ((codValidare & CodEroare.NUME_INCORECT) == CodEroare.NUME_INCORECT)
            {
                lblNume.ForeColor = Color.Red;
            }
            if ((codValidare & CodEroare.PRENUME_INCORECT) == CodEroare.PRENUME_INCORECT)
            {
                lblPrenume.ForeColor = Color.Red;
            }
            if ((codValidare & CodEroare.NOTE_INCORECTE) == CodEroare.NOTE_INCORECTE)
            {
                lblNote.ForeColor = Color.Red;
            }
            if ((codValidare & CodEroare.NO_PROGRAM_STUDIU) == CodEroare.NO_PROGRAM_STUDIU)
            {
                lblSpecializare.ForeColor = Color.Red;
            }
            if ((codValidare & CodEroare.DISCIPLINE_NOTE) == CodEroare.DISCIPLINE_NOTE)
            {
                lblDiscipline.ForeColor = Color.Red;
                lblNote.ForeColor = Color.Red;
            }
        }

        private void ResetCuloareEtichete()
        {
            lblNume.ForeColor = Color.Black;
            lblPrenume.ForeColor = Color.Black;
            lblNote.ForeColor = Color.Black;
            lblSpecializare.ForeColor = Color.Black;
            lblDiscipline.ForeColor = Color.Black;
        }

        private ProgramStudiu GetProgramStudiuSelectat()
        {
            if (rdbCalculatoare.Checked)
                return ProgramStudiu.Calculatoare;
            if (rdbAutomatica.Checked)
                return ProgramStudiu.Automatica;
            if (rdbElectronica.Checked)
                return ProgramStudiu.Electronica;
            if (rdbElectrotehnica.Checked)
                return ProgramStudiu.Electronica;
            if (rdbInginerieE.Checked)
                return ProgramStudiu.InginerieEconomica;
            return ProgramStudiu.Program_Inexistent;
        }

        private void lstAfisare_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetareControale();
            Student s = adminStudenti.GetStudentByIndex(lstAfisare.SelectedIndex-1);

            if (s != null)
            {
                lblID.Text = s.IdStudent.ToString();

                txtNume.Text = s.Nume;
                txtPrenume.Text = s.Prenume;

                foreach (var prgstd in gpbProgrameStudiu.Controls)
                {
                    if (prgstd is RadioButton)
                    {
                        var prgstdBox = prgstd as RadioButton;
                        if (prgstdBox.Text == s.ProgramSTD.ToString())
                        {
                            prgstdBox.Checked = true;
                        }
                    }
                }

                foreach (var disciplina in gpbDiscipline.Controls)
                {
                    if (disciplina is CheckBox)
                    {
                        var disciplinaBox = disciplina as CheckBox;
                        foreach (String dis in s.Discipline)
                            if (disciplinaBox.Text.Equals(dis))
                                disciplinaBox.Checked = true;
                    }
                }

                cmbAnStudiu.Text = s.AnStudiu.ToString();
                txtNote.Text = String.Join(" ",s.GetNote());
            }
        }
    }
}
