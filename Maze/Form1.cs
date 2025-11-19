namespace Maze
{
    public partial class Form1 : Form
    {

        // Vad betyder de olika siffrorna i labyrinten?
        // Istället för att använda "magic numbers" så definierar vi upp
        // dem med namn så att koden blir mer lättläst och lättjobbad.
        const int _empty = 0;
        const int _wall = 1;
        const int _dot = 2;
        const int _man = 3;
        const int _ghost = 4;
        const int _teleportLeft = 5;
        const int _teleportRight = 6;

        // Vilka riktningar motsvarar varje siffra?
        const int _noMove = -1;
        const int _right = 0;
        const int _down = 1;
        const int _left = 2;
        const int _up = 3;

        // Istället för att skriva .GetLength(0) så kan vi enklare se
        // vilken dimension vi vill komma åt
        const int xLength = 1;
        const int yLength = 0;

        // Hur stora grafikblock ska ritas ut? 
        const int blockSize = 32;

        // Uppdateringsfrekvensen för spelet. Anges i millisekunder
        const int gameSpeed = 300;

        // Labyrintdatan. Det här är "originalet" som vi kopierar till en
        // kopia i vilken vi kör själva spelet. På så vis förstörs inte innehållet
        // i labyrinten och vi kan ladda om den när Pacman har ätit alla prickar
        int[,] maze1 =
        {
            {0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0},
            {0,1,2,2,2,2,2,2,2,2,1,2,2,2,2,2,2,2,2,1,0},
            {0,1,2,1,1,2,1,1,1,2,1,2,1,1,1,2,1,1,2,1,0},
            {0,1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1,0},
            {0,1,2,1,1,2,1,2,1,1,1,1,1,2,1,2,1,1,2,1,0},
            {0,1,2,2,2,2,1,2,2,2,1,2,2,2,1,2,2,2,2,1,0},
            {0,1,1,1,1,2,1,1,1,0,1,0,1,1,1,2,1,1,1,1,0},
            {0,0,0,0,1,2,1,0,0,4,0,0,0,0,1,2,1,0,0,0,0},
            {1,1,1,1,1,2,1,0,1,1,1,1,1,0,1,2,1,1,1,1,1},
            {5,0,0,0,0,2,0,0,1,0,0,0,1,0,0,2,0,0,0,0,6},
            {1,1,1,1,1,2,1,0,1,1,1,1,1,0,1,2,1,1,1,1,1},
            {0,0,0,0,1,2,1,0,0,0,0,0,0,0,1,2,1,0,0,0,0},
            {0,1,1,1,1,2,1,0,1,1,1,1,1,0,1,2,1,1,1,1,0},
            {0,1,2,2,2,2,2,2,2,2,1,2,2,2,2,2,2,2,2,1,0},
            {0,1,2,1,1,2,1,1,1,2,1,2,1,1,1,2,1,1,2,1,0},
            {0,1,2,2,1,2,2,2,2,2,3,2,2,2,2,2,1,2,2,1,0},
            {0,1,1,2,1,2,1,2,1,1,1,1,1,2,1,2,1,2,1,1,0},
            {0,1,2,2,2,2,1,2,2,2,1,2,2,2,1,2,2,2,2,1,0},
            {0,1,2,1,1,1,1,1,1,2,1,2,1,1,1,1,1,1,2,1,0},
            {0,1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1,0},
            {0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0},
        };

        // Det här kommer att vara kopian på labyrinten och det är i denna
        // som själva spelet körs
        int[,] maze;

        // Alla spöken i labyrinten kommer att ligga med i denna lista
        // Vi använder en enkel "klass" som bakar samman all data om ett
        // spöke på ett och samma plats.
        List<Ghost> ghosts = new();

        // Allt som behövs för vår Pacman
        int manX = 0;
        int manY = 0;
        int manDirection = _noMove;
        bool manAlive = true;

        int score = 0;

        // Hur många prickar finns i nuvarande labyrint? När denna når noll
        // har Pacman ätit upp alla prickar
        int numDots = 0;


        // Denna metod körs när spelet startar upp
        public Form1()
        {
            // Här startar allt som har med det grafiska gränssnittet upp
            // Knappar, formulär, osv...
            InitializeComponent();

            // När vi startar spelet vill vi dra igång en ny labyrint
            InitMaze();

        }

        // Här skapar vi en kopia av labyrint-datan och lägger till saker som
        // spöken i rätt lista
        private void InitMaze()
        {
            // Hur snabbt ska spelet uppdateras (millisekunder)?
            timer1.Interval = gameSpeed;

            // Det kan finnas spöken kvar från förra banan, de måste rensas bort
            // Annars blir det fler och fler spöken för varje bana
            ghosts.Clear();

            // Nollställ vilken riktning Pacman rör sig i. Den kan vara satt från
            // förra banan och det är viktigt att den nollställs
            manDirection = _noMove;

            // Sätt fönstrets storlek så att det precis täcker hela labyrinten
            this.ClientSize = new System.Drawing.Size(
                maze1.GetLength(xLength) * blockSize,
                maze1.GetLength(yLength) * blockSize
                );

            // Vi skapar en ny labryint som är av samma storlek som originalet
            maze = new int[
                maze1.GetLength(yLength),
                maze1.GetLength(xLength)];

            // Vi går igenom hela labyrinten
            // Den yttre loopen går rad för rad
            // Den inre loopen går kolumn för kolumn
            for (int y = 0; y < maze.GetLength(yLength); y++)
            {
                for (int x = 0; x < maze.GetLength(xLength); x++)
                {
                    // Kopiera datan från orginal-labyrinten till kopian
                    // Det är viktigt att komma ihåg att Y kommer först i vår
                    // labyrint-data!
                    maze[y, x] = maze1[y, x];

                    // Om vi "hittat" på Pacman, så lägger vi in positionen
                    // position i de rätta variablerna
                    if (maze[y, x] == _man)
                    {
                        manX = x;
                        manY = y;
                    }

                    // Om det är ett spöke
                    if (maze[y, x] == _ghost)
                    {
                        // Skapa ett nytt spöke
                        Ghost ghost = new();

                        // Fyll i information om spöket
                        // Position
                        ghost.X = x;
                        ghost.Y = y;

                        // Spöket ska lämna en prick efter sig när det rör sig
                        // för första gången
                        ghost.Leave = _empty;

                        // I vilken riktning ska spöket börja röra sig
                        ghost.Direction = _right;

                        // Lägg till spöket i listan med alla spöken
                        ghosts.Add(ghost);

                        // Eftersom spöket lämnar en prick efter sig måste vi
                        // räkna upp antal prickar

                    }

                    // Om det är en prick så ökar vi på antalet prickar vi funnit
                    // i labyrinten
                    if (maze[y, x] == _dot)
                    {
                        numDots++;
                    }
                }
            }
        }

        // Denna sköter allt som har med utritning att göra
        // Vi kallar aldrig på den här metoden själva
        // Istället körs den när vi gör en: Invalidate()
        protected override void OnPaint(PaintEventArgs e)
        {
            // En SolidBrush definerar vilken färg som ska fyllas i när vi ritar
            SolidBrush wallColor = new SolidBrush(Color.Blue);
            SolidBrush manColor = new SolidBrush(Color.Yellow);
            SolidBrush dotColor = new SolidBrush(Color.White);
            SolidBrush ghostColor = new SolidBrush(Color.Red);

            // Det är viktigt att veta vilket formulär som kallar på denna metod
            // Det vet vi utifrån värdet på variabeln e. Den är en "pekare" som
            // talar om vilket formulär som ledde hit. Vi plockar ut
            // det grafikobjekt som är associerat med forumläret
            Graphics g = e.Graphics;

            // Precis som tidigare kör vi två loopar. En som går igenom varje rad
            // och för varje rad så körs en loop till, som går igenom alla kolumner
            // på den raden
            for (int y = 0; y < maze.GetLength(yLength); y++)
            {
                for (int x = 0; x < maze.GetLength(xLength); x++)
                {
                    // Beroende på vad som ligger i det array-värde som vi läser
                    // ut så ritar vi ut det som passar
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
                        // För att de ska centreras mitt i rutan måste vi
                        // lägga till storleken av ett fjärdedels block
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
                        // Vi ritar spöket i två delar
                        // En cirkel som gör att det blir runt på toppen
                        // och en rektangel som fyller i den nedre halvan
                        // av spöket
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

            // När vi ritat allt som vi vill, låter vi Windows rita alla annat som
            // har med fönstret att göra. T.ex. om vi skulle lagt till knappar eller
            // nåt liknande
            base.OnPaint(e);
        }

        // Om spelaren har tryckt ner en tangent så spar vi undan vilken riktningt
        // som den tangenten motsvarar. Det är först senare, i MoveMan, som 
        // Pacman "rör" på sig.
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // Vi kör WASD-styrning. Man kan lätt lägga till andra
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
            if (manAlive != true)
            {
                if (e.KeyCode == Keys.R)
                {
                    score = 0;
                    this.Text = $"Score: {score}";
                    InitMaze();
                    manAlive = true;

                }
            }
        }

        // Om spelaren släpper upp en tangent så stannar Pacman
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            manDirection = _noMove;
        }

        // Här sker allt som har med Pacman att göra
        private void MoveMan()
        {
            // Vi spar undan var Pacman står när vi kommer in i metoden
            // Då kan vi flytta tillbaka honom om det behövs
            int oldX = manX;
            int oldY = manY;

            // Om Pacman är död så kör vi inte mer av metoden
            if (manAlive != true)
            {
                return;
            }

            // Kolla igenom vilken riktning som blivit satt uppe i KeyDown och
            // rör Pacman i den riktningen
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

            // Om Pacman gått in i en vägg så flyttar vi tillbaka honom
            // till den position han hade när vi kom in i metoden
            if (maze[manY, manX] == _wall)
            {
                manX = oldX;
                manY = oldY;
            }

            // Om Pacman ätit upp en prick
            if (maze[manY, manX] == _dot)
            {
                // Vi får mer poäng
                score++;
                this.Text = $"Score: {score}";

                // Och vi får färre prickar
                numDots--;

                // Har Pacman ätit upp alla prickar?
                if (numDots == 0)
                {
                    // Skapa en ny labyrint
                    InitMaze();

                    // Kör inget mer
                    return;
                }
            }
            // Teleportering
            if (maze[manY, manX] == _teleportLeft)
            {
                manX = 19;
                manY = 9;
            }
            if (maze[manY, manX] == _teleportRight)
            {
                manX = 1;
                manY = 9;
            }

            // Har Pacman gått in i ett spöke så dör han
            if (maze[manY, manX] == _ghost)
            {
                // Ta bort Pacman
                maze[manY, manX] = _empty;
                maze[oldY, oldX] = _empty;

                // Ändra så han är död
                manAlive = false;

                // Kör inget mer
                return;
            }

            // Sätt ut ett tomrum där Pacman stod
            maze[oldY, oldX] = _empty;

            // Sätt ut Pacman på sin nya position
            maze[manY, manX] = _man;
        }
        // Denna metod körs varje gång vår timer räknat ner till 0.
        // Efter att metoden har körts, börjar timern att räkna ner på nytt
        // Allt som har med spelet att göra styrs av denna metod och
        // gör så att allt går i synk med samma hastighet
        private void timer1_Tick(object sender, EventArgs e)
        {
            // Move Pacman first
            MoveMan();

            int width = maze.GetLength(xLength);
            int height = maze.GetLength(yLength);

            for (int i = 0; i < ghosts.Count; i++)
            {
                int oldX = ghosts[i].X;
                int oldY = ghosts[i].Y;

                int gx = oldX;
                int gy = oldY;

                // helpers
                bool InBounds(int x, int y) => x >= 0 && y >= 0 && x < width && y < height;
                bool IsPassable(int x, int y)
                {
                    if (!InBounds(x, y)) return false;
                    int v = maze[y, x];
                    return v != _wall && v != _ghost;
                }

                // reverse dir to penalize U-turns
                int reverse = ghosts[i].Direction switch
                {
                    _up => _down,
                    _down => _up,
                    _left => _right,
                    _right => _left,
                    _ => _noMove
                };

                // candidate moves: (dir, tx, ty)
                var candidates = new List<(int dir, int tx, int ty)>
                {
                    (_up, gx, gy - 1),
                    (_down, gx, gy + 1),
                    (_left, gx - 1, gy),
                    (_right, gx + 1, gy)
                };

                // filter viable moves (allow landing on Pacman or teleports)
                var viable = new List<(int dir, int tx, int ty)>();
                foreach (var c in candidates)
                {
                    if (!InBounds(c.tx, c.ty)) continue;
                    int tile = maze[c.ty, c.tx];
                    if (tile == _man || tile == _teleportLeft || tile == _teleportRight || IsPassable(c.tx, c.ty))
                        viable.Add(c);
                }

                // avoid immediate reverse when multiple choices exist
                if (viable.Count > 1 && reverse != _noMove)
                    viable.RemoveAll(v => v.dir == reverse);

                // if nothing viable, allow reverse if passable
                if (viable.Count == 0 && reverse != _noMove)
                {
                    (int tx, int ty) revTarget = reverse switch
                    {
                        _up => (gx, gy - 1),
                        _down => (gx, gy + 1),
                        _left => (gx - 1, gy),
                        _right => (gx + 1, gy),
                        _ => (gx, gy)
                    };
                    if (InBounds(revTarget.tx, revTarget.ty) && IsPassable(revTarget.tx, revTarget.ty))
                        viable.Add((reverse, revTarget.tx, revTarget.ty));
                }

                // helper: compute final landing coords (consider teleports)
                (int lx, int ly) Landing(int tx, int ty)
                {
                    if (!InBounds(tx, ty)) return (tx, ty);
                    int t = maze[ty, tx];
                    if (t == _teleportLeft) return (19, 9);
                    if (t == _teleportRight) return (1, 9);
                    return (tx, ty);
                }

                // direction priority for deterministic tie-break
                var dirPriority = new Dictionary<int, int> { { _up, 0 }, { _right, 1 }, { _down, 2 }, { _left, 3 } };

                bool moved = false;
                int finalX = gx;
                int finalY = gy;
                int chosenDir = ghosts[i].Direction;

                if (viable.Count > 0)
                {
                    // rank candidates deterministically:
                    // primary = Manhattan distance to Pac-Man after landing (smaller better)
                    // secondary = turn cost (0 = same direction, 1 = turn, 2 = reverse)
                    // tertiary = fixed direction priority
                    double bestScore = double.MaxValue;
                    (int dir, int tx, int ty) best = viable[0];

                    foreach (var v in viable)
                    {
                        var (lx, ly) = Landing(v.tx, v.ty);
                        if (!InBounds(lx, ly)) continue;

                        double dist = Math.Abs(manX - lx) + Math.Abs(manY - ly);

                        int turnCost = v.dir == ghosts[i].Direction ? 0 : (v.dir == reverse ? 2 : 1);

                        int pri = dirPriority.ContainsKey(v.dir) ? dirPriority[v.dir] : int.MaxValue;

                        // combine into tuple-comparable value
                        // use a high weight to keep ordering stable: (dist, turnCost, pri)
                        double score = dist + turnCost * 0.0001 + pri * 0.0000001;

                        if (score < bestScore)
                        {
                            bestScore = score;
                            best = v;
                        }
                    }

                    chosenDir = best.dir;
                    finalX = best.tx;
                    finalY = best.ty;
                    moved = true;
                }

                if (!moved)
                {
                    finalX = oldX;
                    finalY = oldY;
                    chosenDir = ghosts[i].Direction;
                }

                // map teleports to final landing cell
                if (InBounds(finalX, finalY))
                {
                    int landed = maze[finalY, finalX];
                    if (landed == _teleportLeft)
                    {
                        finalX = 19;
                        finalY = 9;
                    }
                    else if (landed == _teleportRight)
                    {
                        finalX = 1;
                        finalY = 9;
                    }
                }

                // restore old cell
                maze[oldY, oldX] = ghosts[i].Leave;

                // avoid collisions with other ghosts
                if (!InBounds(finalX, finalY) || maze[finalY, finalX] == _ghost)
                {
                    finalX = oldX;
                    finalY = oldY;
                }

                // if landing on Pac-Man -> Pac-Man dies
                if (InBounds(finalX, finalY) && maze[finalY, finalX] == _man)
                {
                    manAlive = false;
                    maze[manY, manX] = _empty;
                }

                // remember what was at destination
                ghosts[i].Leave = InBounds(finalX, finalY) ? maze[finalY, finalX] : _empty;

                // update ghost
                ghosts[i].X = finalX;
                ghosts[i].Y = finalY;
                ghosts[i].Direction = chosenDir;

                if (InBounds(finalX, finalY))
                    maze[finalY, finalX] = _ghost;
            }

            // redraw
            Invalidate(true);
        }
    }
}

// Något coolt