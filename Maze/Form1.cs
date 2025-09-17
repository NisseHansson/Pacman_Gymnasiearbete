namespace Maze
{
    public partial class Form1 : Form
    {
        // Vad betyder de olika siffrorna i labyrinten?
        // Ist�llet f�r att anv�nda "magic numbers" s� definierar vi upp
        // dem med namn s� att koden blir mer l�ttl�st och l�ttjobbad.
        const int _empty = 0;
        const int _wall = 1;
        const int _dot = 2;
        const int _man = 3;
        const int _ghost = 4;

        // Vilka riktningar motsvarar varje siffra?
        const int _noMove = -1;
        const int _right = 0;
        const int _down = 1;
        const int _left = 2;
        const int _up = 3;
        const int _lastDir = 4;

        // Ist�llet f�r att skriva .GetLength(0) s� kan vi enklare se
        // vilken dimension vi vill komma �t
        const int xLength = 1;
        const int yLength = 0;

        // Hur stora grafikblock ska ritas ut? 
        const int blockSize = 32;

        // Uppdateringsfrekvensen f�r spelet. Anges i millisekunder
        const int gameSpeed = 300;

        // Labyrintdatan. Det h�r �r "originalet" som vi kopierar till en
        // kopia i vilken vi k�r sj�lva spelet. P� s� vis f�rst�rs inte inneh�llet
        // i labyrinten och vi kan ladda om den n�r Pacman har �tit alla prickar
        int[,] maze1 =
        {
            /*
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, // 1
            {1, 2, 2, 2, 2, 4, 2, 2, 2, 2, 1 }, // 2
            {1, 2, 1, 1, 1, 2, 1, 1, 1, 2, 1 }, // 3
            {1, 2, 1, 2, 2, 2, 2, 2, 1, 2, 1 }, // 4
            {1, 2, 2, 2, 1, 1, 1, 2, 2, 2, 1 }, // 5
            {1, 2, 1, 2, 2, 2, 2, 2, 1, 2, 1 }, // 4
            {1, 2, 1, 1, 1, 2, 1, 1, 1, 2, 1 }, // 3
            {1, 2, 2, 2, 2, 3, 2, 2, 2, 2, 1 }, // 2
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }  // 1
            */
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            {1, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 1},
            {1, 2, 1, 4, 1, 2, 1, 1, 1, 2, 1, 1, 1, 2, 1, 2, 1, 2, 1},
            {1, 2, 1, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 1, 2, 1},
            {1, 2, 1, 1, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 1, 1, 2, 1},
            {1, 2, 2, 2, 1, 2, 2, 2, 1, 2, 1, 2, 2, 2, 1, 2, 2, 2, 1},
            {1, 1, 1, 2, 1, 1, 1, 2, 1, 2, 1, 2, 1, 1, 1, 2, 1, 1, 1},
            {1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1},
            {1, 2, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 2, 1},
            {1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1},
            {1, 1, 1, 2, 1, 1, 1, 2, 1, 2, 1, 2, 1, 1, 1, 2, 1, 1, 1},
            {1, 2, 2, 2, 1, 2, 2, 2, 1, 2, 1, 2, 2, 2, 1, 2, 2, 2, 1},
            {1, 2, 1, 1, 1, 2, 1, 2, 1, 3, 1, 2, 1, 2, 1, 1, 1, 2, 1},
            {1, 2, 1, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 1, 2, 1},
            {1, 2, 1, 2, 1, 2, 1, 1, 1, 2, 1, 1, 1, 2, 1, 2, 1, 2, 1},
            {1, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 1},
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}
        };

        // Det h�r kommer att vara kopian p� labyrinten och det �r i denna
        // som sj�lva spelet k�rs
        int[,] maze;

        // Alla sp�ken i labyrinten kommer att ligga med i denna lista
        // Vi anv�nder en enkel "klass" som bakar samman all data om ett
        // sp�ke p� ett och samma plats.
        List<Ghost> ghosts = new();

        bool WallHit = true;

        // Allt som beh�vs f�r v�r Pacman
        int manX = 0;
        int manY = 0;
        int manDirection = _noMove;
        bool manAlive = true;

        int score = 0;

        // Hur m�nga prickar finns i nuvarande labyrint? N�r denna n�r noll
        // har Pacman �tit upp alla prickar
        int numDots = 0;


        // Denna metod k�rs n�r spelet startar upp
        public Form1()
        {
            // H�r startar allt som har med det grafiska gr�nssnittet upp
            // Knappar, formul�r, osv...
            InitializeComponent();

            // N�r vi startar spelet vill vi dra ig�ng en ny labyrint
            InitMaze();
        }

        // H�r skapar vi en kopia av labyrint-datan och l�gger till saker som
        // sp�ken i r�tt lista
        private void InitMaze()
        {
            // Hur snabbt ska spelet uppdateras (millisekunder)?
            timer1.Interval = gameSpeed;

            // Det kan finnas sp�ken kvar fr�n f�rra banan, de m�ste rensas bort
            // Annars blir det fler och fler sp�ken f�r varje bana
            ghosts.Clear();

            // Nollst�ll vilken riktning Pacman r�r sig i. Den kan vara satt fr�n
            // f�rra banan och det �r viktigt att den nollst�lls
            manDirection = _noMove;

            // S�tt f�nstrets storlek s� att det precis t�cker hela labyrinten
            this.ClientSize = new System.Drawing.Size(
                maze1.GetLength(xLength) * blockSize,
                maze1.GetLength(yLength) * blockSize
                );

            // Vi skapar en ny labryint som �r av samma storlek som originalet
            maze = new int[
                maze1.GetLength(yLength),
                maze1.GetLength(xLength)];

            // Vi g�r igenom hela labyrinten
            // Den yttre loopen g�r rad f�r rad
            // Den inre loopen g�r kolumn f�r kolumn
            for (int y = 0; y < maze.GetLength(yLength); y++)
            {
                for (int x = 0; x < maze.GetLength(xLength); x++)
                {
                    // Kopiera datan fr�n orginal-labyrinten till kopian
                    // Det �r viktigt att komma ih�g att Y kommer f�rst i v�r
                    // labyrint-data!
                    maze[y, x] = maze1[y, x];

                    // Om vi "hittat" p� Pacman, s� l�gger vi in positionen
                    // position i de r�tta variablerna
                    if (maze[y, x] == _man)
                    {
                        manX = x;
                        manY = y;
                    }

                    // Om det �r ett sp�ke
                    if (maze[y, x] == _ghost)
                    {
                        // Skapa ett nytt sp�ke
                        Ghost ghost = new();

                        // Fyll i information om sp�ket
                        // Position
                        ghost.X = x;
                        ghost.Y = y;

                        // Sp�ket ska l�mna en prick efter sig n�r det r�r sig
                        // f�r f�rsta g�ngen
                        ghost.Leave = _dot;

                        // I vilken riktning ska sp�ket b�rja r�ra sig
                        ghost.Direction = _right;

                        // L�gg till sp�ket i listan med alla sp�ken
                        ghosts.Add(ghost);

                        // Eftersom sp�ket l�mnar en prick efter sig m�ste vi
                        // r�kna upp antal prickar
                        numDots++;
                    }

                    // Om det �r en prick s� �kar vi p� antalet prickar vi funnit
                    // i labyrinten
                    if (maze[y, x] == _dot)
                    {
                        numDots++;
                    }
                }
            }
        }

        // Denna sk�ter allt som har med utritning att g�ra
        // Vi kallar aldrig p� den h�r metoden sj�lva
        // Ist�llet k�rs den n�r vi g�r en: Invalidate()
        protected override void OnPaint(PaintEventArgs e)
        {
            // En SolidBrush definerar vilken f�rg som ska fyllas i n�r vi ritar
            SolidBrush wallColor = new SolidBrush(Color.Blue);
            SolidBrush manColor = new SolidBrush(Color.Yellow);
            SolidBrush dotColor = new SolidBrush(Color.White);
            SolidBrush ghostColor = new SolidBrush(Color.Red);

            // Det �r viktigt att veta vilket formul�r som kallar p� denna metod
            // Det vet vi utifr�n v�rdet p� variabeln e. Den �r en "pekare" som
            // talar om vilket formul�r som ledde hit. Vi plockar ut
            // det grafikobjekt som �r associerat med foruml�ret
            Graphics g = e.Graphics;

            // Precis som tidigare k�r vi tv� loopar. En som g�r igenom varje rad
            // och f�r varje rad s� k�rs en loop till, som g�r igenom alla kolumner
            // p� den raden
            for (int y = 0; y < maze.GetLength(yLength); y++)
            {
                for (int x = 0; x < maze.GetLength(xLength); x++)
                {
                    // Beroende p� vad som ligger i det array-v�rde som vi l�ser
                    // ut s� ritar vi ut det som passar
                    if (maze[y, x] == _wall)
                    {
                        g.FillRectangle(
                            wallColor,
                            x * blockSize,
                            y * blockSize,
                            blockSize,
                            blockSize);
                    }

                    if (maze[y, x] == _dot)
                    {
                        // Prickarna ska fylla en halv "ruta"
                        // F�r att de ska centreras mitt i rutan m�ste vi
                        // l�gga till storleken av ett fj�rdedels block
                        g.FillEllipse(
                            dotColor,
                            x * blockSize + blockSize / 4,
                            y * blockSize + blockSize / 4,
                            blockSize / 2,
                            blockSize / 2);
                    }

                    if (maze[y, x] == _man)
                    {
                        g.FillEllipse(
                            manColor,
                            x * blockSize,
                            y * blockSize,
                            blockSize,
                            blockSize);
                    }

                    if (maze[y, x] == _ghost)
                    {
                        // Vi ritar sp�ket i tv� delar
                        // En cirkel som g�r att det blir runt p� toppen
                        // och en rektangel som fyller i den nedre halvan
                        // av sp�ket
                        g.FillEllipse(
                            ghostColor,
                            x * blockSize,
                            y * blockSize,
                            blockSize,
                            blockSize);

                        g.FillRectangle(
                            ghostColor,
                            x * blockSize,
                            y * blockSize + blockSize / 2,
                            blockSize,
                            blockSize / 2);
                    }
                }
            }

            // N�r vi ritat allt som vi vill, l�ter vi Windows rita alla annat som
            // har med f�nstret att g�ra. T.ex. om vi skulle lagt till knappar eller
            // n�t liknande
            base.OnPaint(e);
        }

        // Om spelaren har tryckt ner en tangent s� spar vi undan vilken riktningt
        // som den tangenten motsvarar. Det �r f�rst senare, i MoveMan, som 
        // Pacman "r�r" p� sig.
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // Vi k�r WASD-styrning. Man kan l�tt l�gga till andra
            // typer av styrningar
            if (e.KeyCode == Keys.W)
            {
                manDirection = _up;
            }

            if (e.KeyCode == Keys.S)
            {
                manDirection = _down;
            }

            if (e.KeyCode == Keys.A)
            {
                manDirection = _left;
            }

            if (e.KeyCode == Keys.D)
            {
                manDirection = _right;
            }
        }

        // Om spelaren sl�pper upp en tangent s� stannar Pacman
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            manDirection = _noMove;
        }

        // H�r sker allt som har med Pacman att g�ra
        private void MoveMan()
        {
            // Vi spar undan var Pacman st�r n�r vi kommer in i metoden
            // D� kan vi flytta tillbaka honom om det beh�vs
            int oldX = manX;
            int oldY = manY;

            // Om Pacman �r d�d s� k�r vi inte mer av metoden
            if (manAlive != true)
            {
                return;
            }

            // Kolla igenom vilken riktning som blivit satt uppe i KeyDown och
            // r�r Pacman i den riktningen
            if (manDirection == _right)
            {
                manX++;
            }

            if (manDirection == _down)
            {
                manY++;
            }

            if (manDirection == _left)
            {
                manX--;
            }

            if (manDirection == _up)
            {
                manY--;
            }

            // Om Pacman g�tt in i en v�gg s� flyttar vi tillbaka honom
            // till den position han hade n�r vi kom in i metoden
            if (maze[manY, manX] == _wall)
            {
                manX = oldX;
                manY = oldY;
            }

            // Om Pacman �tit upp en prick
            if (maze[manY, manX] == _dot)
            {
                // Vi f�r mer po�ng
                score++;
                this.Text = $"Score: {score}";

                // Och vi f�r f�rre prickar
                numDots--;

                // Har Pacman �tit upp alla prickar?
                if (numDots == 0)
                {
                    // Skapa en ny labyrint
                    InitMaze();

                    // K�r inget mer
                    return;
                }
            }

            // Har Pacman g�tt in i ett sp�ke s� d�r han
            if (maze[manY, manX] == _ghost)
            {
                // Ta bort Pacman
                maze[manY, manX] = _empty;
                maze[oldY, oldX] = _empty;

                // �ndra s� han �r d�d
                manAlive = false;

                // K�r inget mer
                return;
            }

            // S�tt ut ett tomrum d�r Pacman stod
            maze[oldY, oldX] = _empty;

            // S�tt ut Pacman p� sin nya position
            maze[manY, manX] = _man;
        }

        // Denna metod k�rs varje g�ng v�r timer r�knat ner till 0.
        // Efter att metoden har k�rts, b�rjar timern att r�kna ner p� nytt
        // Allt som har med spelet att g�ra styrs av denna metod och
        // g�r s� att allt g�r i synk med samma hastighet
        private void timer1_Tick(object sender, EventArgs e)
        {
            // Flytta p� Pacman
            MoveMan();

            // G� igenom alla sp�ken som finns i ghosts-listan
            for (int i = 0; i < ghosts.Count; i++)
            {
                // Spara undan sp�kets nuvarande position
                int oldX = ghosts[i].X;
                int oldY = ghosts[i].Y;
                /*
                if (WallHit)
                {
                    if (Math.Abs(manX - ghosts[i].X) < Math.Abs(manY - ghosts[i].Y))
                    {
                        if (manY - ghosts[i].Y < 0)
                        {
                            ghosts[i].Y--;
                            ghosts[i].Direction = _up;
                        }
                        else
                        {
                            ghosts[i].Y++;
                            ghosts[i].Direction = _down;
                        }
                        if (maze[ghosts[i].Y, ghosts[i].X] == _wall ||
                            maze[ghosts[i].Y, ghosts[i].X] == _ghost)
                        {


                            if (manX - ghosts[i].X < 0)
                            {

                                ghosts[i].Direction = _left;
                            }
                            else
                            {

                                ghosts[i].Direction = _right;
                            }
                        }

                    }
                    else
                    {
                        if (manX - ghosts[i].X < 0)
                        {
                            ghosts[i].X--;
                            ghosts[i].Direction = _left;
                        }
                        else
                        {
                            ghosts[i].X++;
                            ghosts[i].Direction = _right;
                        }
                        if (maze[ghosts[i].Y, ghosts[i].X] == _wall ||
                            maze[ghosts[i].Y, ghosts[i].X] == _ghost)
                        {

                            if (manY - ghosts[i].Y < 0)
                            {

                                ghosts[i].Direction = _up;
                            }
                            else
                            {

                                ghosts[i].Direction = _down;
                            }


                        }
                        ghosts[i].X = oldX;
                        ghosts[i].Y = oldY;
                        WallHit = false;
                    }
                    if (ghosts[i].Direction == _right)
                    {
                        ghosts[i].X++;
                    }

                    if (ghosts[i].Direction == _down)
                    {
                        ghosts[i].Y++;
                    }

                    if (ghosts[i].Direction == _left)
                    {
                        ghosts[i].X--;
                    }

                    if (ghosts[i].Direction == _up)
                    {
                        ghosts[i].Y--;
                    }
                    if (maze[ghosts[i].Y, ghosts[i].X] == _wall ||
                       maze[ghosts[i].Y, ghosts[i].X] == _ghost)
                    {
                        WallHit = true;
                    }
                */

                if (Math.Abs(manX - ghosts[i].X) < Math.Abs(manY - ghosts[i].Y))
                {
                    if (manY - ghosts[i].Y < 0)
                    {
                        ghosts[i].Y--;
                        ghosts[i].Direction = _up;
                    }
                    else
                    {
                        ghosts[i].Y++;
                        ghosts[i].Direction = _down;

                    }

                    if (maze[ghosts[i].Y, ghosts[i].X] == _wall ||
                        maze[ghosts[i].Y, ghosts[i].X] == _ghost)
                    {
                        ghosts[i].X = oldX;
                        ghosts[i].Y = oldY;
                        if (manX - ghosts[i].X < 0)
                        {
                            ghosts[i].X--;
                            ghosts[i].Direction = _left;
                        }
                        else
                        {
                            ghosts[i].X++;
                            ghosts[i].Direction = _right;
                        }
                    }
                }
                else
                {
                    if (manX - ghosts[i].X < 0)
                    {
                        ghosts[i].X--;
                        ghosts[i].Direction = _left;
                    }
                    else
                    {
                        ghosts[i].X++;
                        ghosts[i].Direction = _right;
                    }
                    if (maze[ghosts[i].Y, ghosts[i].X] == _wall ||
                    maze[ghosts[i].Y, ghosts[i].X] == _ghost)
                    {
                        ghosts[i].X = oldX;
                        ghosts[i].Y = oldY;
                        if (manY - ghosts[i].Y < 0)
                        {
                            ghosts[i].Y--;
                            ghosts[i].Direction = _up;

                        }
                        else
                        {
                            ghosts[i].Y++;
                            ghosts[i].Direction = _down;
                        }
                    }
                }


                // Kolla igenom vilken riktning sp�ket �r p� v�g och 
                // f�rflytta det enligt riktningen
                /*
                if (ghosts[i].Direction == _right)
                {
                    ghosts[i].X++;
                }

                if (ghosts[i].Direction == _down)
                {
                    ghosts[i].Y++;
                }

                if (ghosts[i].Direction == _left)
                {
                    ghosts[i].X--;
                }

                if (ghosts[i].Direction == _up)
                {
                    ghosts[i].Y--;
                }
                */
                // Har sp�ket g�tt in i n�got som det inte ska kunna passera
                // igenom? Just nu kollar vi efter v�ggar och andra sp�ken

                if (maze[ghosts[i].Y, ghosts[i].X] == _wall ||
                    maze[ghosts[i].Y, ghosts[i].X] == _ghost)
                {
                    // Flytta tillbaka sp�ket dit d�r det stod i b�rjan av
                    // loopen
                    ghosts[i].X = oldX;
                    ghosts[i].Y = oldY;
                    /*
                    // Sv�ng �t h�ger
                    ghosts[i].Direction++;

                    // Om vi �r p� v�g upp�t och sv�nger h�ger, s�
                    // ska vi b�rja om med riktningen h�ger
                    if (ghosts[i].Direction >= _lastDir)
                    {
                        ghosts[i].Direction = _right;
                    }
                    */
                }
                

                    // Om vi g�tt p� Pacman, ska han d�
                    if (maze[ghosts[i].Y, ghosts[i].X] == _man)
                    {
                        maze[manY, manX] = _empty;
                        maze[oldY, oldX] = _empty;

                        manAlive = false;
                    }

                    // Vi l�gger tillbaka det som l�g p� sp�kets gamla position
                    // Detta g�rs f�r att saker som prickar inte ska f�rsvinna nr
                    // sp�kena passerar �ver
                    maze[oldY, oldX] = ghosts[i].Leave;

                    // Kom ih�g vad som l�g p� den position sp�ket hamnar p�
                    // Det kommer att l�ggas tillbaka n�sta g�ng metoden k�rs
                    ghosts[i].Leave = maze[ghosts[i].Y, ghosts[i].X];

                    // L�gg in sp�ket p� dess nya position
                    maze[ghosts[i].Y, ghosts[i].X] = _ghost;
                }

                // Allt som har med spelet att g�ras �r uppdaterat s� det �r
                // dags att rita om allt
                Invalidate(true);

            }
        }
    }


// N�got coolt