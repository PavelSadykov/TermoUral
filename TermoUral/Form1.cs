using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace TermoUral
{

    


    public partial class Form1 : Form
    {
        double M;//заданный заливочный вес изделия, кг
        double MA;//заданный заливочный вес компонента А,кг
        double MB; //заданный заливочный вес компонента В,кг
        double MAfact;// Вес компонента A при заливе за 1 операцию , кг
        double MBfact;// Вес компонента В при заливе за 1 операцию , кг
        double Mfact;// Суммарный вес компонентов при заливе за 1 операцию , кг
        double MAnotFilled;// Вес компонента A для дозалива изделия, кг
        double MBnotFilled;// Вес компонента В для дозалива изделия,кг
        double MnotFilled;// Вес  для дозалива изделия,кг
        double ComponentRatio;// Соотношение компонентов В/А
        double DensityA;//плотность компонента А кг/ литр
        double DensityB;//плотность компонента B кг/ литр
        double DensityA1;//плотность компонента А кг/ m3
        double DensityB1;//плотность компонента B кг/ m3
        double PumpCapacityA;//производительность насоса А , cm3/об
        double PumpCapacityB;//производительность насоса B , cm3/об
        double ComponentTimeStart;//время старта компонента, сек 
        double MArev;//заливочный А вес кг/об
        double MBrev;//заливочный В вес кг/об
        double MBSec;//Расход В кг/сек
        double MASec;//Расход A кг/сек
        double Msec; // Расход суммарный А и В кг/сек
        double Dout;// Диаметр наружной оболочки, мм
        double Dpipe;// Диаметр трубы, мм
        double SizePipe;//Длина трубы, м
        double D1out;// Диаметр наружной оболочки, м
        double D1pipe;// Диаметр трубы, м
        double SizePipe1;//Длина трубы за минусом не изолированных концов, м
        double RotationSetB;// обороты в минуту В
        double RotationSetA;// обороты в минуту А
        double RotationSetB1;// обороты в секунду В
        double RotationSetA1;// обороты в секунду А
        double RevRatio;// соотношение оборотов В/А
        double TimeFillingPipe;// Заданное время на издлелие, сек
        double TimeFillingPipe1;// Время за 1 операцию, сек 
        double TimeFillingPipe2;// Скорректированное время в условиях пограничных состояний, сек
        double NumRevPumpВ;// Заданное количчество оборотов В на изделие 
        double NumRevPumpA;// Заданное количчество оборотов А на изделие 
        double NumRevPumpB1;//текущая переменная
        double NumRevPumpA1;//текущая переменная
        double NumRevPumpВ2;//Количество оборотов В за 1 операцию
        double NumRevPumpA2;//Количество оборотов А за 1 операцию
        double MBfact1;//Скорректированный вес В в условиях пограничных состояний, кг
        double MAfact1;//Скорректированный вес А в условиях пограничных состояний, кг
        double Mfact1;//Скорректированный суммарный вес в условиях пограничных состояний, кг
        double MBfact2;// Итоговый вес заливки В, кг
        double MAfact2;// Итоговый вес заливки А, кг
        double Mfact2;// Итоговый вес заливки изделия, кг


        public Form1()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            MAfact = 0; MBfact = 0; Mfact = 0; MAnotFilled = 0; MAnotFilled = 0; MBnotFilled = 0;
            MnotFilled = 0; TimeFillingPipe = 0; TimeFillingPipe1 = 0; MBfact1 = 0; MAfact1 = 0;
            Mfact1 = 0; MBfact2 = 0; MAfact2 = 0; Mfact2 = 0;

            string inputText = textBoxInput.Text;
            if (double.TryParse(inputText, out M))
            {
                MessageBox.Show("Значение М: " + M);
            }
            else
            {
                MessageBox.Show("Неверный формат числа ");
            }
            DensityA = 1.09;
            DensityB = 1.24;
            PumpCapacityA = 58.7;
            PumpCapacityB = 54.9;
            ComponentRatio = 1.8;
            ComponentTimeStart = 40.0;

            MA = M / (ComponentRatio + 1);//
            MB = (M * ComponentRatio) / (1 + ComponentRatio);//
            DensityA1 = DensityA / 1000;
            DensityB1 = DensityB / 1000;
            MArev = PumpCapacityA * DensityA1;
            MBrev = PumpCapacityB * DensityB1;
            NumRevPumpA = MA / MArev;
            NumRevPumpВ = MB / MBrev;
            RevRatio = NumRevPumpВ / NumRevPumpA;//
           

            if (M > 54)
            {
                RotationSetB1 = 24;
            }
            else
            {
                RotationSetB1 = 14.1;
            }

            if (RotationSetB1 / RevRatio > 24)
            {
                RotationSetA1 = 24;
            }
            else
            {
                RotationSetA1 = RotationSetB1 / RevRatio;
            }

            RotationSetA = RotationSetA1 * 60;//
            RotationSetB = RotationSetB1 * 60;//

            if (M > 54)
            {
                TimeFillingPipe = NumRevPumpВ / 24;// расчетное время заливки если вес > 54
                if (TimeFillingPipe < ComponentTimeStart)//1440 обороты
                {
                    TimeFillingPipe1 = NumRevPumpВ / 24;// установленное время заливки , если  меньше 40

                }
                else
                {
                    TimeFillingPipe1 = ComponentTimeStart;//установленное время если  40
                }
            }
            else if(M <= 54)
            {
                TimeFillingPipe = NumRevPumpВ / 14.1;//расчетное время заливки если вес < 54
                if (TimeFillingPipe < ComponentTimeStart)//846 обороты
                {
                    TimeFillingPipe1 = NumRevPumpВ / 14.1;//время меньше 40
                }
                else
                {
                    TimeFillingPipe1 = ComponentTimeStart;// время 40
                }
            }
            
            if (TimeFillingPipe >= ComponentTimeStart)// если общее время заливки больше 40, проверяем на сколько превышает вес.
            {
                NumRevPumpB1 = ComponentTimeStart * RotationSetB1; // определяем сколь нужно оборотов B

                NumRevPumpA1 = ComponentTimeStart * RotationSetA1; // определяем сколь нужно оборотов A

                MBfact2 = NumRevPumpB1 * MBrev;// определяем вес B

                MAfact2 = NumRevPumpA1 * MArev;// определяем вес A

                Mfact2 = MAfact2 + MBfact2;//

                MBfact = MBfact2;
                MAfact = MAfact2;
                Mfact = MAfact + MBfact;
                MAnotFilled = MA - MAfact2;
                MBnotFilled = MB - MBfact2;
                MnotFilled = MAnotFilled + MBnotFilled;// превышенный вес
              

            }
            else if (TimeFillingPipe < ComponentTimeStart)
            {
                NumRevPumpB1 = TimeFillingPipe * RotationSetB1; 
                                                                // определяем сколь нужно оборотов B             

                NumRevPumpA1 = TimeFillingPipe * RotationSetA1; // определяем сколь нужно оборотов A
           
                MBfact2 = NumRevPumpB1 * MBrev;// определяем вес B
                MAfact2 = NumRevPumpA1 * MArev;// определяем вес A

                Mfact2 = MAfact2 + MBfact2;//общий вес

                MBfact = RotationSetB1 * MBrev * TimeFillingPipe1;
                MAfact = RotationSetA1 * MArev * TimeFillingPipe1;
                Mfact = MAfact + MBfact;
                MAnotFilled = MA - MAfact2;
                MBnotFilled = MB - MBfact2;
                MnotFilled = MAnotFilled + MBnotFilled;// превышенный вес
              

            }
            if((MnotFilled < 5) & (MnotFilled > 0.01))// корректируем заливочный вес .
                {
                Mfact1 = M - 5;
                MBfact1 = (Mfact1 * ComponentRatio) / (1 + ComponentRatio);//
                MAfact1 = Mfact1 / (1 + ComponentRatio);//
                MBfact = MBfact1;
                MAfact = MAfact1;
                Mfact = Mfact1;

                //корректируем кол-во оборотов
                NumRevPumpВ2 = MBfact / MBrev; //  обороты B на 1 заливку
                NumRevPumpA2 = MAfact / MArev; //  обороты A на 1 заливку

                //корректируем время заливки
                TimeFillingPipe2 = NumRevPumpВ2 / 24;
                TimeFillingPipe1 = TimeFillingPipe2;
                //корректируем оставшиййся вес на следующую заливку
                MAnotFilled = MA - MAfact;
                MBnotFilled = MB - MBfact;
                MnotFilled = MAnotFilled + MBnotFilled;// 
                //корректируем для отображения на экране
                MBfact2 = MBfact;
                MAfact2 = MAfact;
                Mfact2 = Mfact;

            }
            else
            {
                Mfact1 = 0;
                MBfact1 = 0;//
                MAfact1 = 0;
                NumRevPumpВ2 = MBfact / MBrev; //  обороты B на 1 заливку
                NumRevPumpA2 = MAfact / MArev; //  обороты A на 1 заливку
                TimeFillingPipe2 = 0;
            }

            MBSec = MBrev * RotationSetB1;
            MASec = MArev * RotationSetA1;
            Msec = MBSec + MASec;

            //Отображение на экране
            textBoxRotationSetB.Text = $"{RotationSetB}";
            textBoxRotationSetA.Text = $"{Math.Round(RotationSetA,0)}";
            textBoxDensityB.Text = $"{DensityB1}";
            textBoxDensityA.Text = $"{DensityA1}";
            textBoxMB.Text = $"{Math.Round(MB,1)}";
            textBoxM.Text = $"{M}";
            textBoxMA.Text = $"{Math.Round(MA,1)}";
            textBoxMBfact1.Text = $"{Math.Round(MBfact1, 2)}";
            textBoxMfact1.Text = $"{Math.Round(Mfact1, 2)}";
            textBoxMAfact1.Text = $"{Math.Round(MAfact1, 2)}";
            textBoxMBfact.Text = $"{Math.Round(MBfact, 2)}";
            textBoxMfact.Text = $"{Math.Round(Mfact, 2)}";
            textBoxMAfact.Text = $"{Math.Round(MAfact, 2)}";
            textBoxMBrev.Text = $"{MBrev}";
            textBoxMArev.Text = $"{MArev}";
            textBoxMBSec.Text = $"{MBSec}";
            textBoxMASec.Text = $"{MASec}";
            textBoxMsec.Text = $"{Msec}";
            textBoxNumRevPumpB.Text = $"{Math.Round(NumRevPumpВ,1)}";
            textBoxNumRevPumpA.Text = $"{Math.Round(NumRevPumpA,1)}";
            textBoxTimeFillingPipe.Text = $"{Math.Round(TimeFillingPipe,2)}";
            textBoxTimeFilPipe1.Text = $"{Math.Round(TimeFillingPipe1,2)}";
            textBoxTimeFillingPipe2.Text = $"{Math.Round(TimeFillingPipe2,2)}";
            textBoxRotationSetB1.Text = $"{RotationSetB1}";
            textBoxRotationSetA1.Text = $"{Math.Round(RotationSetA1,1)}";
            textBoxRevRatio.Text = $"{Math.Round(RevRatio,2)}";
            textBoxComponentRatio.Text = $"{Math.Round(ComponentRatio, 2)}";
            textBoxMBfact2.Text = $"{Math.Round(MBfact2, 2)}";
            textBoxMfact2.Text = $"{Math.Round(Mfact2, 2)}";
            textBoxMAfact2.Text = $"{Math.Round(MAfact2, 2)}";
            textBoxMBnotFilled.Text = $"{Math.Round(MBnotFilled, 2)}";
            textBoxMnotFilled.Text = $"{Math.Round(MnotFilled, 2)}";
            textBoxMAnotFilled.Text = $"{Math.Round(MAnotFilled, 2)}";
            textBoxNumRevPumpB1.Text = $"{Math.Round(NumRevPumpB1, 1)}";
            textBoxNumRevPumpA1.Text = $"{Math.Round(NumRevPumpA1, 1)}";
            textBoxNumRevPumpB2.Text = $"{Math.Round(NumRevPumpВ2, 1)}";
            textBoxNumRevPumpA2.Text = $"{Math.Round(NumRevPumpA2, 1)}";


        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {




        }

        private void textBoxRotationSetB_TextChanged(object sender, EventArgs e)
        {
           
        }




    }
 }




