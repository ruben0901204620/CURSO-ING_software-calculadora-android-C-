using System;
using Android.App;
using Android.OS;
using Android.Widget;
using Android.Graphics;
using Android.Views.Animations; 
using AndroidX.AppCompat.App;

namespace Calculadora
{
    [Activity(Label = "Calculadora",
        Icon = "@drawable/icon_app",
        RoundIcon = "@drawable/icon_circular",
        Theme = "@style/AppTheme", 
        MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        TextView txtResult, txtOperation;
        LinearLayout mainLayout;
        Button btnTheme;
        string currentInput = "";
        string lastOperator = "";
        double firstValue = 0;
        bool isNewEntry = true;
        bool isDarkMode = true;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            txtResult = FindViewById<TextView>(Resource.Id.txtResult);
            txtOperation = FindViewById<TextView>(Resource.Id.txtOperation);
            mainLayout = FindViewById<LinearLayout>(Resource.Id.mainLayout);
            btnTheme = FindViewById<Button>(Resource.Id.btnTheme);

            // --- LOGICA DE TEMA ---
            btnTheme.Click += (s, e) => {
                AnimarBoton((Button)s); // Animación al cambiar tema
                isDarkMode = !isDarkMode;
                mainLayout.SetBackgroundColor(isDarkMode ? Color.Black : Color.White);
                txtResult.SetTextColor(isDarkMode ? Color.White : Color.Black);
                txtOperation.SetTextColor(isDarkMode ? Color.Gray : Color.DarkGray);
                btnTheme.Text = isDarkMode ? "☀" : "🌙";
                btnTheme.SetTextColor(isDarkMode ? Color.White : Color.Black);
            };

            // --- NUMEROS ---
            int[] numbers = { Resource.Id.btn0, Resource.Id.btn1, Resource.Id.btn2, Resource.Id.btn3,
                             Resource.Id.btn4, Resource.Id.btn5, Resource.Id.btn6, Resource.Id.btn7,
                             Resource.Id.btn8, Resource.Id.btn9, Resource.Id.btnDot };

            foreach (int id in numbers)
            {
                FindViewById<Button>(id).Click += (s, e) => {
                    var btn = (Button)s;
                    AnimarBoton(btn); 
                    string val = btn.Text;
                    if (isNewEntry) { currentInput = ""; isNewEntry = false; }
                    if (val == "." && currentInput.Contains(".")) return;
                    currentInput += val;
                    txtResult.Text = currentInput;
                };
            }

            // --- OPERADORES ---
            int[] ops = { Resource.Id.btnAdd, Resource.Id.btnSub, Resource.Id.btnMult, Resource.Id.btnDiv };
            foreach (int id in ops)
            {
                FindViewById<Button>(id).Click += (s, e) => {
                    var btn = (Button)s;
                    AnimarBoton(btn); 
                    if (double.TryParse(currentInput, out double val))
                    {
                        firstValue = val;
                        lastOperator = btn.Text;
                        txtOperation.Text = $"{firstValue} {lastOperator}";
                        isNewEntry = true;
                    }
                };
            }

            // --- BOTONES ESPECIALES (+/-, %, C, =) ---
            ConfigurarBotonEspecial(Resource.Id.btnPlusMinus, () => {
                if (double.TryParse(currentInput, out double val))
                {
                    currentInput = (val * -1).ToString();
                    txtResult.Text = currentInput;
                }
            });

            ConfigurarBotonEspecial(Resource.Id.btnPercent, () => {
                if (double.TryParse(currentInput, out double val))
                {
                    currentInput = (val / 100).ToString();
                    txtResult.Text = currentInput;
                    isNewEntry = true;
                }
            });

            ConfigurarBotonEspecial(Resource.Id.btnC, () => {
                currentInput = ""; firstValue = 0; lastOperator = "";
                txtResult.Text = "0"; txtOperation.Text = ""; isNewEntry = true;
            });

            ConfigurarBotonEspecial(Resource.Id.btnEqual, () => {
                if (double.TryParse(currentInput, out double secondValue))
                {
                    double res = 0;
                    switch (lastOperator)
                    {
                        case "+": res = firstValue + secondValue; break;
                        case "-": res = firstValue - secondValue; break;
                        case "×": res = firstValue * secondValue; break;
                        case "÷": res = secondValue != 0 ? firstValue / secondValue : 0; break;
                        default: return;
                    }
                    txtResult.Text = res.ToString();
                    txtOperation.Text = "";
                    currentInput = res.ToString();
                    isNewEntry = true;
                }
            });
        }

        // Metodo auxiliar para no repetir codigo en botones especiales
        private void ConfigurarBotonEspecial(int id, Action accion)
        {
            var btn = FindViewById<Button>(id);
            btn.Click += (s, e) => {
                AnimarBoton(btn);
                accion();
            };
        }

        // --- ANIMACION ---
        private void AnimarBoton(Button btn)
        {
            ScaleAnimation anim = new ScaleAnimation(0.9f, 1.0f, 0.9f, 1.0f,
                Dimension.RelativeToSelf, 0.5f, Dimension.RelativeToSelf, 0.5f);
            anim.Duration = 100; 
            btn.StartAnimation(anim);
        }
    }
}