using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace BubbleBreakerGame
{
    public partial class frmBubbleBreaker : Form
    {
        enum Colors
        {
            None,
            Red,
            Green,
            Yellow,
            Blue,
            Purple
        };
        

        const int NUM_BUBBLES = 5;
        const int BUBBLE_SIZE = 40;
        Colors[,] colors;
        Random rand;
        int score;
        bool[,] isSelected;
        int numOfSelectedBubbles;
        Scores scores;


        public frmBubbleBreaker()
        {
            InitializeComponent();
            rand = new Random();
            numOfSelectedBubbles = 0;
            score = 0;
            colors = new Colors[NUM_BUBBLES, NUM_BUBBLES]; // Array ul pentru culori
            isSelected = new bool[NUM_BUBBLES, NUM_BUBBLES]; // Tine bulinele de aceeasi culoare selectate de user
            lblInfo.BackColor = Color.White; // Culoarea de background a label-ului
        }

        private void frmBubbleBreaker_Load(object sender, EventArgs e)
        {
            init();
        }

        private void init()
        {
            SetClientSizeCore(NUM_BUBBLES * BUBBLE_SIZE, NUM_BUBBLES * BUBBLE_SIZE);
            FormBorderStyle = FormBorderStyle.FixedSingle; // Opreste user-ul in a da resize la window
            MaximizeBox = false; // Opreste user-ul in a maximiza window-ul
            BackColor = Color.Black;
            DoubleBuffered = true; 

            txtName.Visible = false;
            btnName.Visible = false;
            btnName.Text = "Enter Your Name";

            txtName.Width = ClientSize.Width < 100 ? ClientSize.Width : ClientSize.Width / 2;
            btnName.Width = ClientSize.Width < 100 ? ClientSize.Width : ClientSize.Width / 2;
            txtName.Location = new Point((ClientSize.Width - txtName.Width) / 2, txtName.Height);
            btnName.Location = new Point((ClientSize.Width - btnName.Width) / 2, btnName.Height + 20);
            Start();
        }

        private void GameOver()
        {
            scores = new Scores(score, NUM_BUBBLES);
            StringBuilder sb = new StringBuilder();
            sb.Append("***GAME OVER***");
            sb.Append("\n");
            sb.Append("***TOP 3 SCORES***\n");
            sb.Append(scores.GetTopThreeScores() + "\n");
            sb.Append("\n");
            sb.Append(scores.GetScoreMessage());

            MessageBox.Show(sb.ToString());
            txtName.Visible = true;
            btnName.Visible = true;
        }

        // popularea array-ului cu buline de culori random
        private void Start()
        {
            for (int row = 0; row < NUM_BUBBLES; row++)
            {
                for (int col = 0; col < NUM_BUBBLES; col++)
                {
                    colors[row, col] = (Colors) rand.Next(1, 6); // Array ul primeste o culoare de la 1 la 5 inafara de none
                }
            }

            this.Text = score + " points"; // Afisarea scorului initial
        }

        //Coloreaza bulinele si realizeaza o margine pentru bulinele selectate
        private void Form_Paint(object sender, PaintEventArgs e)
        {
            for (int row = 0; row < NUM_BUBBLES; row++)
            {
                for(int col = 0; col < NUM_BUBBLES; col++)
                {
                    Color bubbleColor = Color.Empty;
                    var xPos = col;
                    var yPos = row;
                    var isBubble = true;

                    switch (colors[row, col]) // verifica ce culoare este in array
                    {
                        case Colors.Red:
                            bubbleColor = Color.Red;
                            break;
                        case Colors.Yellow:
                            bubbleColor = Color.Yellow;
                            break;
                        case Colors.Green:
                            bubbleColor = Color.Green;
                            break;
                        case Colors.Blue:
                            bubbleColor = Color.Blue;
                            break;
                        case Colors.Purple:
                            bubbleColor = Color.Purple;
                            break;
                        default:
                            e.Graphics.FillRectangle(Brushes.Black, xPos * BUBBLE_SIZE, 
                                yPos * BUBBLE_SIZE, BUBBLE_SIZE, BUBBLE_SIZE);
                            isBubble = false;
                            break;
                    }

                    if (isBubble) 
                    {
                        e.Graphics.FillEllipse(
                        new LinearGradientBrush(
                        new Point(row * BUBBLE_SIZE, col * BUBBLE_SIZE),
                        new Point(row * BUBBLE_SIZE + BUBBLE_SIZE, col * BUBBLE_SIZE + BUBBLE_SIZE),
                        bubbleColor, bubbleColor), // bubbleColor fara gradient, Color.White cu gradient
                        xPos * BUBBLE_SIZE, 
                        yPos * BUBBLE_SIZE, 
                        BUBBLE_SIZE, BUBBLE_SIZE); 

                        if (isSelected[row, col])
                        {
                            // stanga
                            if (col > 0 && colors[row, col] != colors[row, col - 1])
                                e.Graphics.DrawLine(Pens.White, xPos * BUBBLE_SIZE, yPos * BUBBLE_SIZE,
                                    xPos * BUBBLE_SIZE, yPos * BUBBLE_SIZE + BUBBLE_SIZE);

                            // dreapta
                            if (col < NUM_BUBBLES - 1 && colors[row, col] != colors[row, col + 1])
                                e.Graphics.DrawLine(Pens.White, xPos * BUBBLE_SIZE + BUBBLE_SIZE, yPos * BUBBLE_SIZE,
                                    xPos * BUBBLE_SIZE + BUBBLE_SIZE, yPos * BUBBLE_SIZE + BUBBLE_SIZE);

                            // top
                            if (row > 0 && colors[row, col] != colors[row - 1, col])
                                e.Graphics.DrawLine(Pens.White, xPos * BUBBLE_SIZE, yPos * BUBBLE_SIZE,
                                    xPos * BUBBLE_SIZE + BUBBLE_SIZE, yPos * BUBBLE_SIZE);

                            // bot
                            if (row < NUM_BUBBLES - 1 && colors[row, col] != colors[row + 1, col])
                                e.Graphics.DrawLine(Pens.White, xPos * BUBBLE_SIZE, yPos * BUBBLE_SIZE + BUBBLE_SIZE,
                                    xPos * BUBBLE_SIZE + BUBBLE_SIZE, yPos * BUBBLE_SIZE + BUBBLE_SIZE);
                        }
                    }                    
                }
            }
        }
        // mouse click event. Daca nu este selectata nicio bulina, click-ul selecteaza bulinele.
        // Daca bulinele sunt selectate, click-ul le sterge din window.
        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            // coordonatele bulinelor unde a fost pus click-ul convertite in coordonatele folosite pentru a desena obiectele
            var x = Convert.ToInt32 (e.X / BUBBLE_SIZE);
            var y = Convert.ToInt32 (e.Y / BUBBLE_SIZE);

            // randul si coloana bulinelor selectate in array
            var row = y;
            var col = x;

            if (isSelected[row,col] && numOfSelectedBubbles > 1) // User ul a dat click pe coordonate si sunt mai mult de 1
            {
                score += Convert.ToInt32(lblInfo.Text); // Daca sunt mai multe, updatam scorul
                this.Text = score + " points";
                RemoveBubbles();
                ClearSelected();
                MoveBubblesDown();
                MoveBubblesRight();

                if (!HasMoreMoves()) // Daca nu mai sunt miscari, jocul se incheie
                {
                    GameOver();
                }
            }
            else
            {
                ClearSelected();

                if (colors[row, col] > Colors.None)
                {
                    HighlightNeighbors(row, col);
                    this.Invalidate();
                    Application.DoEvents();

                    if (numOfSelectedBubbles > 1)
                    {
                        SetLabel(numOfSelectedBubbles, x, y);
                    }
                }
            }
        }

        // sterge bulinele dandu-le bulinelor sterge culoarea "none"
        private void RemoveBubbles()
        {
            for (int row = 0; row < NUM_BUBBLES; row++)
            {
                for (int col = 0; col < NUM_BUBBLES; col++)
                {
                    if (isSelected[row, col])
                        colors[row, col] = Colors.None;
                }
            }

            this.Invalidate();
            Application.DoEvents();
        }

        // sterge bulinele selectate din array
        private void ClearSelected()
        {
            for (int row = 0; row < NUM_BUBBLES; row++)
            {
                for (int col = 0; col < NUM_BUBBLES; col++)
                {
                    isSelected[row, col] = false;
                }
            }

            numOfSelectedBubbles = 0;
            lblInfo.Visible = false;
        }

        // verifica daca jocul s-a terminat. Jocul nu s-a terminat daca mai sunt macar 2 buline vecine cu aceeasi culoare
        private bool HasMoreMoves()
        {
            for (int row = 0; row < NUM_BUBBLES; row++)
            {
                for (int col = 0; col < NUM_BUBBLES; col++)
                {
                    if (colors[row, col] > Colors.None)
                    {
                        if (col < NUM_BUBBLES - 1 && colors[row, col] == colors[row, col + 1])
                            return true;

                        if (row < NUM_BUBBLES - 1 && colors[row, col] == colors[row + 1, col])
                            return true;
                    }
                }
            }

            return false;
        }

        // Label-ul cu scorul pentru bulinele selectate
        private void SetLabel(int numOfBubles, int x, int y)
        {
            var value = numOfBubles * (numOfBubles - 1); // daca sunt selectate 2 buline * 3 puncte = scor 6 etc etc
            lblInfo.Text = value.ToString();

            lblInfo.Left = x * BUBBLE_SIZE + BUBBLE_SIZE;
            lblInfo.Top = y * BUBBLE_SIZE + BUBBLE_SIZE;

            // daca label-ul este prea aproape de margine sau iese din cadru, se misca in putin in cadru
            if (lblInfo.Left > this.ClientSize.Width / 2)
                lblInfo.Left -= BUBBLE_SIZE;

            if (lblInfo.Top > this.ClientSize.Height / 2)
                lblInfo.Top -= BUBBLE_SIZE;

            lblInfo.Visible = true; // visible property
        }

        // muta bulinele jos in locurile unde au fost sterse bulinele anterioare
        private void MoveBubblesDown()
        {
            for (int col = 0; col < NUM_BUBBLES; col++)
            {
                var noneColorBubblePosition = NUM_BUBBLES - 1;
                var foundNoneColor = false;

                for (int row = NUM_BUBBLES - 1; row >= 0; row--)
                {
                    // gaseste pozitia primei buline sterse
                    if (colors[row, col] == Colors.None)
                        foundNoneColor = true;

                    if (colors[row, col] != Colors.None && !foundNoneColor)
                        noneColorBubblePosition--;

                    if (colors[row,col] != Colors.None && foundNoneColor)
                    {
                        colors[noneColorBubblePosition, col] = colors[row, col];
                        noneColorBubblePosition--;
                    }                    
                }
                //sterge bulinele ramase deasupra
                for (int r = noneColorBubblePosition; r >= 0; r--)
                {
                    colors[r, col] = Colors.None;
                }
            }

            this.Invalidate();
            Application.DoEvents();
        }

        // dupa ce bulinele se muta jos, muta bulinele in stanga pentru a umple orice loc creat de bulinele sterse
        private void MoveBubblesRight()
        {
            for (int row = 0; row < NUM_BUBBLES; row++)
            {
                var noneColorBubblePosition = NUM_BUBBLES - 1;
                var foundNoneColor = false;

                for (int col = NUM_BUBBLES - 1; col >= 0; col--)
                {
                    // gaseste pozitia primei buline sterse
                    if (colors[row, col] == Colors.None) 
                        foundNoneColor = true;

                    if (colors[row, col] != Colors.None && !foundNoneColor)
                        noneColorBubblePosition--;

                    //start moving bubbles to the right on positions of the removed bubbles
                    if (colors[row, col] != Colors.None && foundNoneColor)
                    {
                        colors[row, noneColorBubblePosition] = colors[row, col];
                        noneColorBubblePosition--;
                    }
                }

                // sterge bulinele ramase in stanga
                for (int c = noneColorBubblePosition; c >= 0; c--)
                {
                    colors[row, c] = Colors.None;
                }
            }

            this.Invalidate();
            Application.DoEvents();
            GenerateBubbles();
        }

        private void GenerateBubbles()
        {
            // Daca coltul din stanga jos are culoarea "none" ca si proprietate, atunci generam o bulina
            if (colors[NUM_BUBBLES -1, 0] == Colors.None)
            {
                for (int row = NUM_BUBBLES - 1; row >= 0; row--)
                {
                    colors[row, 0] = (Colors) rand.Next(1, 6);
                }

                this.Invalidate();
                Application.DoEvents();
                MoveBubblesRight();
            }
        }
        // selecteaza bulinele vecine cu aceeasi culoare
        private void HighlightNeighbors(int row, int col)
        {

            isSelected[row, col] = true;
            numOfSelectedBubbles++;

            //up
            if (row > 0 && colors[row, col] == colors[row - 1, col] &&
                !isSelected[row - 1, col])
            {
                HighlightNeighbors(row - 1, col);
            }

            //down
            if (row < NUM_BUBBLES - 1 && colors[row, col] == colors[row + 1, col] &&
                !isSelected[row + 1, col])
            {
                HighlightNeighbors(row + 1, col);
            }

            //stanga
            if (col > 0 && colors[row, col] == colors[row, col - 1] &&
                !isSelected[row, col - 1])
            {
                HighlightNeighbors(row, col - 1);
            }

            //dreapta
            if (col < NUM_BUBBLES - 1 && colors[row, col] == colors[row, col + 1] &&
                !isSelected[row, col + 1])
            {
                HighlightNeighbors(row, col + 1);
            }

        }

        private void btnName_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter a valid name");
                return;
            }

            scores.WriteScore(txtName.Text);
            numOfSelectedBubbles = 0;
            txtName.Text = "";
            score = 0;
            init();
        }
    }
}
