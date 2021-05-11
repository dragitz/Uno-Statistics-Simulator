using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Uno_Statistics_Simulator
{



    public partial class Code : Form
    {
        /*

      WHAT WE WANT

      This project will allow us to analyze the probability of winning in a game and give us useful info.

      { FEATURES }
      -Provide info on probabilities of getting a card
      -Provide info on counted cards
      -Provide info on probability of winning
      -Provide info on best move


      { SYSTEM }

      TO DO: (ordered by priority)

      -Setup arrays of cards
      -Scoring system        
      -Rules
      -Shuffle system for the pile of cards

      -Count cards

      -Risk system
      -AI
      -Play best move
      -Automatically order people's decks






      */

        public Code()
        {
            InitializeComponent();
        }


        async Task PutTaskDelay(int time)
        {
            await Task.Delay(time);
        }



        //                                                  Global variables, YAY !

        public static class Globals
        {
            //testing stuff here...........
            public static double red = 0.25;
            public static double green = 0.25;
            public static double blue = 0.25;
            public static double yellow = 0.25;

            public static string the_last = "";






            //Debugging, stepping through each decision,....
            public static bool DebugWait = false; //   If 1=Game must wait      0=Game is automatic

            public static Int32 FailSafe = 0;  //   If 1=Simulation can not be executed because of invalid card distribution      0=Game can start!
                                               //   NOTE: I've added this failsafe just in case, but it should never fail!

            public static Int32 NotEnoughCards = 0;  //   If 1=Simulation can not be executed because there are not enough cards      0=Game can start!


            public static long watch = 0;      //Time taken to simulate


            //Scoring
            //Note: https://www.unorules.com/

            public static Int32 Score_Traps = 20;    // +2, Skip, Reverse
            public static Int32 Score_Wilds = 50;    // Any wild card
            public static Int32 Score_Specials = 40; // Special cards



            //Score of current playing player
            public static Int32 Score = 0;

            public static Int32 PlayerTurn = 1;

            public static bool isGameOn = false;




            //RULES

            //if player has at leat one playable set this flag to true.
            public static bool CanPlay = true;

            //If player doesn't have any available cards to play, then set this flag to true
            public static bool ShouldPick = true;

            //Will check if player must switch cards
            public static bool MustSwitch = true;

            //If player can respond to a trap/wild card.
            public static bool CanRespondPlay = true;

            //Used to skip "CanRespondPlay" check, for eg. you can respond to a Skip with a Skip.
            public static bool DisableResponse = true;

            //Will determine the next player ( aka victim >:] )
            public static bool Clockwise = true;


            //PLAYER RULES
            public static bool DoIHaveSameColors = false;
            public static bool DoIHaveSameNumbers = false;
            public static bool DoIHaveSameID = false;
            public static bool DoIHaveWild = false;




            //Players deck (arrays)
            // 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, Plus2, Plus4, Skip, Reverse, ChangeColor, FindZero, Double, Switch
            // +
            // _COLOR
            public static string[] P1Deck = new string[] { "" };
            public static string[] P2Deck = new string[] { "" };
            public static string[] P3Deck = new string[] { "" };
            public static string[] P4Deck = new string[] { "" };
            public static string[] P5Deck = new string[] { "" };
            public static string[] P6Deck = new string[] { "" };
            public static string[] P7Deck = new string[] { "" };
            public static string[] P8Deck = new string[] { "" };


            //The amount of cards each player is holding
            public static Int32 P1Deck_Count = 0;
            public static Int32 P2Deck_Count = 0;
            public static Int32 P3Deck_Count = 0;
            public static Int32 P4Deck_Count = 0;
            public static Int32 P5Deck_Count = 0;
            public static Int32 P6Deck_Count = 0;
            public static Int32 P7Deck_Count = 0;
            public static Int32 P8Deck_Count = 0;

            // Used to count cards, assuming every player knows how to count cards, this is tecnically shared. (Good thing)
            public static string[] P1Deck_NoCards = new string[] { "" };
            public static string[] P2Deck_NoCards = new string[] { "" };
            public static string[] P3Deck_NoCards = new string[] { "" };
            public static string[] P4Deck_NoCards = new string[] { "" };
            public static string[] P5Deck_NoCards = new string[] { "" };
            public static string[] P6Deck_NoCards = new string[] { "" };
            public static string[] P7Deck_NoCards = new string[] { "" };
            public static string[] P8Deck_NoCards = new string[] { "" };



            //The following array will be used to "remember" which card have been recently played, 10 cards should be enough, as humans don't remember much.
            //Number of maximum cards to remember can be edited in the settings tab.

            //NOTE: I will also add a slider that will incluence the probability of remembering a card.
            //Eg. if the value of the slider is set to 0.1, it means:
            //Top card: 100% probability of remembering
            //2nd card: 90%
            //3rd card: 80% .... and so on.
            //The slider should influence the probability. <--- this might be a big bias, so a re-run with different values should help remove it
            //
            //DEV NOTE: NEED TO IMPLEMENT A SEED, SO THE SAME SHUFFLE CAN BE REPEATED WITH DIFFERENT SETTINGS.
            public static string[] TableArray = new string[] { "" };

            //Card info
            public static string[] TYPES = new string[] { "Number", "Trap", "Wild", "Special" };

            public static string[] COLORS = new string[] { "Red", "Green", "Blue", "Yellow", "None" };

            public static string[] NUMBERS = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

            public static string[] TRAPS = new string[] { "Plus2", "Skip", "Reverse" };

            public static string[] WILDS = new string[] { "Plus4", "ChangeColor", "FindZero", "Double", "Switch" };





            //                  {{{{{{{{     TABLE INFO     }}}}}}}}

            //This part will be ultra dynamic, it will represent the whole game, what's on the table, what and how many cards the player is holding (enemy included)

            // Color of the top card on the table
            public static string Color = "";

            // Type of the top card on the table (Number, Trap, Wild)
            public static string CardType = "";

            // The format of the top card on the table:
            //
            // Eg: 0_Green     --->  Green card 0
            // Eg: Plus4_None  --->  +4 wild
            //
            public static string CardID = "";



            //Amount of cards in the pile
            public static Int32 CardsAmount = 0; //Amount of cards is the sum of all cards in the settings

            //Amount of cards in the pile
            public static Int32 CardCount_Me = 0; //Amount of card the current player is holding
            public static Int32 CardCount_Next = 0; //Amount of card the next player is holding





            //Card ids will be added to this array.
            public static string[] Pile = new string[] { "" };

            //The id of the card array
            public static Int32 Pile_ID = 0;

            //This will store every single info about the game
            public static dynamic JsonGame = "";

            //Will store every info about all simulated games into a Json format, can also be converted into a .csv format
            /*
            Json format will be like this:
            
            Games
                ID_OF_GAME
                        Winner: //Who won
                        Score:  //Score of winner
                        Seed:   //Seed of the game (unsure how to save it, as rng is outside of window range)

            */
            public static dynamic JsonFinalResultsOfSimulation = "";

        }




        //                                 {{{{{{{{     RUN SIMULATION     }}}}}}}}



        private void Run_Click(object sender, System.EventArgs e)
        {

            NewGame();

            ExecuteTimer.Start();
            ExecuteTimer.Tick += Play;
        }



        public async void Play(object sender, EventArgs e)
        {
            ExecuteTimer.Stop();

            //Globals.FailSafe = 1; //Force failsafe

            //Check for failsafe
            if (Globals.PlayerTurn == 0 || Globals.FailSafe == 1 || Globals.PlayerTurn > PlayersAmount.Maximum || PlayersAmount.Value == 0 || Globals.NotEnoughCards == 1)
            {
                GamesAmount.Value = GamesAmount.Minimum;
                DebugBox.Text = "";

                Status.Text = "Status: Simulation Ended  ---->  Error (either the card distribution is wrong or memory is corrupted) check debug box";

                DebugBox.AppendText("PlayerTurn ----> " + Globals.PlayerTurn + Environment.NewLine);
                if (Globals.FailSafe == 1) { DebugBox.AppendText("FailSafe is set to 1 ---> Try using a set seed." + Environment.NewLine); }
                if (Globals.PlayerTurn > PlayersAmount.Maximum) { DebugBox.AppendText("PlayerTurn > PlayersAmount.Maximum ----> Are you editing memory? Wrong value to edit" + Environment.NewLine); }
                if (PlayersAmount.Value == 0) { DebugBox.AppendText("PlayersAmount.Value == 0 ----> Are you editing memory? Wrong value to edit" + Environment.NewLine); }
                if (Globals.NotEnoughCards == 1) { DebugBox.AppendText("Globals.NotEnoughCards ---->  You need to add more cards" + Environment.NewLine); }

                DebugBox.AppendText(Environment.NewLine + "Open an issue on my GitHub and paste this output if you don't know how to solve this issue");
            }
            else
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();

                Status.Text = "Status: Running simulation...";
                //var TempPlayerArray[] =




                //Alpha is the id of the game being simulated
                for (int alpha = 1; alpha <= GamesAmount.Value; alpha++)
                {

                    NewGame();
                    //Set game to on
                    Globals.isGameOn = true;

                    //Used to check if player can play or not
                    //var ALLOW_TURN_EDIT = 1;

                    //Display current game in Main 
                    CurrentGameLabel.Text = "Current game: " + alpha + "/" + GamesAmount.Value;



                    //While game is on
                    //                           {{{{{{{{     NOTE: THIS IS BASICALLY THE BEGINNING OF THE AI     }}}}}}}}
                    while (Globals.isGameOn)
                    {
                        //Add a tiny-tiny delay
                        await PutTaskDelay(1);

                        //Reset personal flags
                        Globals.DoIHaveSameColors = false;
                        Globals.DoIHaveSameNumbers = false;
                        Globals.DoIHaveSameID = false;
                        Globals.DoIHaveWild = false;

                        //Fix stepping, as it doesn't really work.
                        if (Stepping.Checked)
                        {
                            SpinWait.SpinUntil(() => Globals.DebugWait == false, 50); //This can be used to slow down the calculation
                            if (Globals.DebugWait == false)
                            {

                                Globals.DebugWait = true;
                                Globals.isGameOn = false;
                            }

                        }


                        /*
                        
                        We should consider a setting up a risk score first, then choose what to play based on that

                        But before that, check who's playing next.


                        */




                        //                                 {{{{{{{{     EVALUATION     }}}}}}}}




                        //                                 {{{{{{{{     WHO'S NEXT ?     }}}}}}}}
                        JObject rss = JObject.Parse(Globals.JsonGame);
                        JObject game = rss["Game"] as JObject;

                        //Who's playing next?
                        // Json: Game --> Next_Player
                        Globals.PlayerTurn = (int)rss["Game"]["Next_Player"];

                        if (Globals.Clockwise == true)
                        {


                            if (Globals.PlayerTurn + 1 > PlayersAmount.Value)
                            {
                                game["Next_Player"] = 1;
                                Globals.PlayerTurn = 1;
                            }
                            else
                            {
                                game["Next_Player"] = Globals.PlayerTurn + 1;
                                Globals.PlayerTurn = Globals.PlayerTurn + 1;

                            }

                        }
                        else
                        {

                            if (Globals.PlayerTurn - 1 == 0)
                            {
                                game["Next_Player"] = PlayersAmount.Value;
                                Globals.PlayerTurn = Convert.ToInt32(PlayersAmount.Value);

                            }
                            else
                            {
                                game["Next_Player"] = Globals.PlayerTurn - 1;
                                Globals.PlayerTurn = Globals.PlayerTurn - 1;

                            }

                        }


                        //                                 {{{{{{{{     GET TOP CARD     }}}}}}}}
                        //Get top card

                        Scoreboard.Text = Globals.JsonGame; //debug

                        Globals.DoIHaveSameColors = false;
                        Globals.DoIHaveSameID = false;

                        //Get identity of deck (current player turn)
                        JArray decks = (JArray)game["Player_Decks"][Globals.PlayerTurn.ToString()];

                        //Check if I have the same color
                        string color_to_check = (string)rss["Game"]["TopCard_Color"];
                        for (int i = 0; i < decks.Count; i++)
                        {
                            if (Globals.DoIHaveSameColors == false && color_to_check == decks[i].ToString().Split('_').Last())
                            {

                                //Set top card color
                                //Top should be: 0_Green
                                //Seed 0 -->  Deck: Contains 1_Green. so it's true (last is 9_blue)

                                Globals.DoIHaveSameColors = true;

                                //Scoreboard.Text = "Top: "+color_to_check+ ",   my: "+decks[i].ToString().Split('_').Last(); //debug
                            }
                        }


                        //Check if I have the same id
                        //It can be a number, trap, wild, ecc.. Same card ignoring color
                        string id_to_check = (string)rss["Game"]["TopCard_ID"];
                        for (int i = 0; i < decks.Count; i++)
                        {
                            if (Globals.DoIHaveSameID == false && id_to_check == decks[i].ToString().Split('_').First())
                            {
                                if (decks[i].ToString().Split('_').First() == "Wild")
                                {

                                    Globals.DoIHaveWild = true;
                                }
                                Globals.DoIHaveSameID = true;
                            }
                        }






                        /* Calculate end game
                         
                        -We can check if any player has no cards, that means someone has won
                                Better add this check after a placement/usage of card and then we set a flag or simply set "isGameOn" to true

                         */



                        //Atm I have no idea if the turn change works, as there's nothing that checks if a game must go on.
                        //Once code gets executed here, game just ends (thinking of including a score check for "isGameOn")

                        //Change turn

                        //End of game
                        if (Stepping.Checked == false)
                        {
                            Globals.isGameOn = false;
                        }
                    }



                }
                Status.Text = "Status: Simulation Ended";

                // the code that you want to measure comes here
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                EmulationLabel.Text = "Emulation time taken: " + elapsedMs.ToString() + "ms";
            }

            //END OF SIMULATION


        }





        private void NewGame()
        {
            /*

           TODO:
           Force Reset game -----------

           Read settings and add cards to pile --------------
           Shuffle pile ----------------------
           Give cards to players ------------------
           Place first top pile card in the middle


           Randomize starting player? (Add this option to remove bias and keep track of individual wins)

           Run Ai
               -Infinite pile?
               -Bottom card of pile must be reset


           */






            //                                 {{{{{{{{     RESET SETTINGS     }}}}}}}}



            //Reset global decks (will free some memory)
            Globals.P1Deck = new string[] { "" };
            Globals.P2Deck = new string[] { "" };
            Globals.P3Deck = new string[] { "" };
            Globals.P4Deck = new string[] { "" };
            Globals.P5Deck = new string[] { "" };
            Globals.P6Deck = new string[] { "" };
            Globals.P7Deck = new string[] { "" };
            Globals.P8Deck = new string[] { "" };

            //Reset table
            Globals.Pile = new string[] { "" }; //    NOTE:   Card format   -->  CardID_Color

            Globals.NotEnoughCards = 0;
            Globals.FailSafe = 0;

            Globals.Clockwise = true;           //Wich direction the game is going

            Globals.PlayerTurn = 1;             //Reset player turn


            /*
            Explaining the structure:

            -Risk[]         = Contains the risk% for each card in the current player's hand. The higer the number, the lower% it will be played.
                              Will be reset and edited every turn.
            
            -Player_Decks{} = Contains data (card_id) about player decks
            -Deck_NoCards{} = Contains data (card_id) about the last 3-5 cards played by the player, used to count cards.
            -Pile[]         = All cards remaining
            */
            Globals.JsonGame = "";
            string json = @"{
                              'Game': {
                                'TopCard':'',
                                'TopCard_ID':'',
                                'TopCard_Color':'',
                                'TopCard_Type':'',                                
                                'Next_Player':1,                                
                                
                                'Color_Probability':{
                                    '1': [
                                        0.25,
                                        0.25,
                                        0.25,
                                        0.25
                                    ],
                                    '2': [
                                        0.25,
                                        0.25,
                                        0.25,
                                        0.25
                                    ],
                                    '3': [
                                        0.25,
                                        0.25,
                                        0.25,
                                        0.25
                                    ],
                                    '4': [
                                        0.25,
                                        0.25,
                                        0.25,
                                        0.25
                                    ],
                                    '5': [
                                        0.25,
                                        0.25,
                                        0.25,
                                        0.25
                                    ],
                                    '6': [
                                        0.25,
                                        0.25,
                                        0.25,
                                        0.25
                                    ],
                                    '7': [
                                        0.25,
                                        0.25,
                                        0.25,
                                        0.25
                                    ],
                                    '8': [
                                        0.25,
                                        0.25,
                                        0.25,
                                        0.25
                                    ]
                                },
                                'Player_Decks':{
                                    '1': [],
                                    '2': [],
                                    '3': [],
                                    '4': [],
                                    '5': [],
                                    '6': [],
                                    '7': [],
                                    '8': []
                                },
                                'Deck_NoCards':{
                                    '1': [],
                                    '2': [],
                                    '3': [],
                                    '4': [],
                                    '5': [],
                                    '6': [],
                                    '7': [],
                                    '8': []
                                },
                                'Pile':[]
                              }
                            }";






            //Then the actual array of cards.   NOTE: It will be shuffled later
            //NOTE:   0, 1, 2, 3, 4, 5, 6, 7, 8, 9, Plus2, Plus4, ChangeColor, FindZero, Double, Switch
            //NOTE:   Card format   -->  CardID_Color

            //Numbers 1-9
            for (int k = 0; k < NumOfNumberCards.Value; k++)
            {
                for (int COLOR_NUMB = 0; COLOR_NUMB < 4; COLOR_NUMB++) //Set to 4 because we need to ignore the "None" color (aka. wild cards)
                {
                    for (int i = 1; i <= 9; i++)
                    {
                        Array.Resize(ref Globals.Pile, Globals.Pile.Length + 1);
                        Globals.Pile[Globals.Pile.Length - 1] = i + "_" + Globals.COLORS[COLOR_NUMB];

                    }
                }
            }


            //Zeros
            for (int k = 0; k < NumOFZeros.Value; k++)
            {
                for (int COLOR_NUMB = 0; COLOR_NUMB < 4; COLOR_NUMB++) //Set to 4 because we need to ignore the "None" color (aka. wild cards)
                {
                    Array.Resize(ref Globals.Pile, Globals.Pile.Length + 1);
                    Globals.Pile[Globals.Pile.Length - 1] = "0_" + Globals.COLORS[COLOR_NUMB];
                }

            }

            //Skip turn cards
            for (int k = 0; k < NumOfSkips.Value; k++)
            {
                for (int COLOR_NUMB = 0; COLOR_NUMB < 4; COLOR_NUMB++) //Set to 4 because we need to ignore the "None" color (aka. wild cards)
                {
                    Array.Resize(ref Globals.Pile, Globals.Pile.Length + 1);
                    Globals.Pile[Globals.Pile.Length - 1] = "Skip_" + Globals.COLORS[COLOR_NUMB];
                }

            }

            //Reverse turn cards
            for (int k = 0; k < NumOfReverses.Value; k++)
            {
                for (int COLOR_NUMB = 0; COLOR_NUMB < 4; COLOR_NUMB++) //Set to 4 because we need to ignore the "None" color (aka. wild cards)
                {
                    Array.Resize(ref Globals.Pile, Globals.Pile.Length + 1);
                    Globals.Pile[Globals.Pile.Length - 1] = "Reverse_" + Globals.COLORS[COLOR_NUMB];
                }

            }

            //+2 cards
            for (int k = 0; k < NumOfDrawPlus2.Value; k++)
            {
                for (int COLOR_NUMB = 0; COLOR_NUMB < 4; COLOR_NUMB++) //Set to 4 because we need to ignore the "None" color (aka. wild cards)
                {
                    Array.Resize(ref Globals.Pile, Globals.Pile.Length + 1);
                    Globals.Pile[Globals.Pile.Length - 1] = "Plus2_" + Globals.COLORS[COLOR_NUMB];
                }

            }


            //Wild change color cards
            for (int k = 0; k < NumOfWild.Value; k++)
            {
                Array.Resize(ref Globals.Pile, Globals.Pile.Length + 1);
                Globals.Pile[Globals.Pile.Length - 1] = "ChangeColor_None";

            }

            //Wild +4  cards
            for (int k = 0; k < NumOfWildPlus4.Value; k++)
            {
                Array.Resize(ref Globals.Pile, Globals.Pile.Length + 1);
                Globals.Pile[Globals.Pile.Length - 1] = "Plus4_None";

            }

            //Special cards (switch, find zero, double)
            for (int k = 0; k < NumOfSpecial.Value; k++)
            {
                if (FindZero.Checked)
                {
                    Array.Resize(ref Globals.Pile, Globals.Pile.Length + 1);
                    Globals.Pile[Globals.Pile.Length - 1] = "FindZero_None";
                }
                if (Double.Checked)
                {
                    Array.Resize(ref Globals.Pile, Globals.Pile.Length + 1);
                    Globals.Pile[Globals.Pile.Length - 1] = "Double_None";
                }
                if (Double.Checked)
                {
                    Array.Resize(ref Globals.Pile, Globals.Pile.Length + 1);
                    Globals.Pile[Globals.Pile.Length - 1] = "Switch_None";
                }
            }


            Globals.Pile = Globals.Pile = Globals.Pile.Skip(1).ToArray();

            PileCountLabel.Text = "Pile Count: " + Globals.Pile.Length;



            if (Globals.Pile.Length / PlayersAmount.Value < 14 || Globals.Pile.Length / PlayersAmount.Value - PlayerInitialCards.Value < 10)
            {
                Globals.NotEnoughCards = 1;
            }
            else
            {
                //SHUFFLE PILE
                if (DisableSeedGen.Checked)
                {
                    //Thanks Joel for the regex code! https://stackoverflow.com/a/262466/13741865

                    Regex digitsOnly = new Regex(@"[^\d]");
                    SeedBox.Text = digitsOnly.Replace(SeedBox.Text, "0");

                    //Set seed
                    var rng = new Random(Convert.ToInt32(SeedBox.Text));

                    rng.Shuffle(Globals.Pile);
                    rng.Shuffle(Globals.Pile); // different order from first call to Shuffle
                }
                else
                {
                    var rng = new Random();
                    rng.Shuffle(Globals.Pile);
                    rng.Shuffle(Globals.Pile); // different order from first call to Shuffle
                }




                //                                 {{{{{{{{     DISTRIBUTE CARDS TO PLAYERS     }}}}}}}}




                int cards_given = 0;
                int cards_to_give = Decimal.ToInt32(PlayersAmount.Value * PlayerInitialCards.Value);


                string[] aaaaa = new string[] { "" };

                JObject rss = JObject.Parse(json);

                //PILE OF CARDS TO JSON
                //Create json basic structure

                /// aaaaa is a temp array
                aaaaa = Globals.Pile;
                for (int k = 0; k < Globals.Pile.Length; k++)
                {

                    JObject game = rss["Game"] as JObject;

                    JArray item = (JArray)game["Pile"];
                    item.Add(aaaaa[k]);


                }

                //CARD DISTRIBUTION
                for (int i = 1; i <= PlayersAmount.Value; i++)
                {

                    for (int k = 0; k < PlayerInitialCards.Value; k++)
                    {
                        //  Give card from pile to player's deck
                        cards_given++;
                        JObject game = rss["Game"]["Player_Decks"] as JObject;
                        JArray item = (JArray)game[i.ToString()];
                        item.Add(Globals.Pile[cards_given]);

                        //  Remove given card id from pile
                        game = rss["Game"] as JObject;
                        item = (JArray)game["Pile"];
                        item.RemoveAt(cards_given);                       // Id of card to remove

                    }

                }

                // DEV NOTE: Dunno why it doesn't work without the if. I'll probably forget about this.
                if (true)
                {
                    //Place top card from pile to table
                    //Copy top card from pile to TopCard (game table)

                    JObject game = rss["Game"] as JObject;
                    JArray item = (JArray)game["Pile"];
                    string test = item[0].ToString();

                    //Remove from pile
                    item = (JArray)game["Pile"];
                    item.RemoveAt(0);                       // Id of card to remove

                    //Set top card
                    game["TopCard"] = test;

                    //Set top card color
                    game["TopCard_Color"] = test.Split('_').Last();

                    //Set card ID
                    game["TopCard_ID"] = test.Split('_').First();



                    //First (half) evaluation


                    //Check if TopCard is a number
                    if (test.Split(new Char[] { '_' })[0].All(Char.IsDigit) == true)
                    {
                        game["TopCard_Type"] = "Number";
                    }

                    //Check if TopCard is a  wild
                    if (test.Split('_').Last() == "None")
                    {
                        //Set top card color
                        game["TopCard_Type"] = "Wild";
                    }

                    //Check if TopCard is a Special
                    if (test.Contains("Switch") || test.Contains("FindZero") || test.Contains("Double"))
                    {
                        //Set top card color
                        game["TopCard_Type"] = "Special";
                    }

                    //Check if TopCard is a Trap
                    if (test.Contains("Reverse") || test.Contains("Plus2") || test.Contains("Skip"))
                    {
                        //Set top card color
                        game["TopCard_Type"] = "Trap";
                    }



                }



                //Save initial card list here
                //DEV NOTE: ALWAYS SAVE AFTER MAKING EDITS!

                Globals.JsonGame = rss.ToString();

                //Scoreboard.Text = Globals.JsonGame;



                //Globals.FailSafe = 1; //Force failsafe
                //DEV NOTE: TO DO ---> add the rng seed to the message
                if (cards_given != cards_to_give)
                {

                    Globals.FailSafe = 1;
                    DebugBox.Text = "";
                    DebugBox.Text = "FAILSAFE CAN NOT CONTINUE. CARDS ARE NOT EQUALLY DISTRIBUTED " + cards_given + "/" + cards_to_give;

                }

            }
            //                                 {{{{{{{{     END OF DISTRIBUTION     }}}}}}}}
        }

























        //                              General stuff. Ignore unless adding new features.






        private void trackBar1_Scroll(object sender, System.EventArgs e)
        {
            label10.Text = "Player cards: " + PlayerInitialCards.Value;
        }

        private void trackBar2_Scroll(object sender, System.EventArgs e)
        {
            label11.Text = "Win score: " + WinScore.Value;
        }



        //Reset settings
        private void DefaultPileSettings_Click(object sender, System.EventArgs e)
        {
            //Reset card count
            NumOfNumberCards.Value = 2;
            NumOFZeros.Value = 2;
            NumOfSkips.Value = 2;
            NumOfReverses.Value = 2;
            NumOfDrawPlus2.Value = 2;
            NumOfWild.Value = 2;
            NumOfWildPlus4.Value = 2;
            NumOfSpecial.Value = 3;

            //Other settings
            PlayerInitialCards.Value = 7;
            WinScore.Value = 100;

            label10.Text = "Player cards: " + PlayerInitialCards.Value;
            label11.Text = "Win score: " + WinScore.Value;

        }

        private void Stepping_CheckedChanged(object sender, EventArgs e)
        {
            if (Stepping.Checked == true)
            {
                Globals.DebugWait = true;
                NextStep.Enabled = true;
                NextStep2.Enabled = true;
            }
            else
            {
                Globals.DebugWait = false;
                NextStep.Enabled = false;
                NextStep2.Enabled = false;
            }

        }

        private void AllowMultipleCards_MouseHover(object sender, EventArgs e)
        {
            ToolTip tt = new ToolTip();
            tt.SetToolTip(AllowMultipleCards, "If you have multiple same cards (color doesn't matter), you can place them all.");
        }

        private void FindZero_MouseHover(object sender, EventArgs e)
        {
            ToolTip tt = new ToolTip();
            tt.SetToolTip(FindZero, "The player who places the card can pick a color and then the next player has to draw cards until he finds a Zero or draws 10 cards.");
        }

        private void Double_MouseHover(object sender, EventArgs e)
        {
            ToolTip tt = new ToolTip();
            tt.SetToolTip(Double, "The player who places the card can pick a color and then next player has to draw as many cards as he has.");
        }

        private void SwitchChards_MouseHover(object sender, EventArgs e)
        {
            ToolTip tt = new ToolTip();
            tt.SetToolTip(SwitchChards, "The player who places the card can pick a color and then will swap his hand with the next player.");
        }

        private void PrintToDebug_MouseHover(object sender, EventArgs e)
        {
            ToolTip tt = new ToolTip();
            tt.SetToolTip(PrintToDebug, "Prints the \"thinking\" process of the Ai.");
        }

        private void ThinkingLabel_MouseHover(object sender, EventArgs e)
        {
            ToolTip tt = new ToolTip();
            tt.SetToolTip(ThinkingLabel, "Low end PCs might need a higher number.");
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            ThinkTimer.Interval = Decimal.ToInt32(TimeMS.Value);
        }

        private void NextStep_Click(object sender, EventArgs e)
        {
            Globals.DebugWait = false;
        }

        private void NextStep2_Click(object sender, EventArgs e)
        {
            Globals.DebugWait = false;
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            GamesAmount.Value = GamesAmount.Minimum;
        }










        private void reset_test_Click(object sender, EventArgs e)
        {
            NewGame();

            suggested_test.Text = "Suggested play: Not enough info";
            Globals.red = 0.25;
            Globals.green = 0.25;
            Globals.blue = 0.25;
            Globals.yellow = 0.25;

            label15.Text = "Red: " + Globals.red * 100 + "%     Blue: " + Globals.blue * 100 + "%     Green: " + Globals.green * 100 + "%     Yellow: " + Globals.yellow * 100 + "%       Somma= " + (Globals.red + Globals.blue + Globals.green + Globals.yellow) * 100;
        }




        private void place_red_Click(object sender, EventArgs e)
        {
            CardProbability("red", 1, false);
        }

        private void button3_Click(object sender, EventArgs e)
        {

            CardProbability("blue", 1, false);
        }

        private void green_test_Click(object sender, EventArgs e)
        {
            CardProbability("green", 1, false);
        }

        private void place_yellow_Click(object sender, EventArgs e)
        {
            CardProbability("yellow", 1, false);
        }
        private void CardProbability(string card_color, int id_turn, bool picked)
        {
            //Get probability
            JObject rss = JObject.Parse(Globals.JsonGame);

            JObject game = rss["Game"]["Color_Probability"] as JObject;
            JArray item = (JArray)game[id_turn.ToString()];

            if (picked == false) { Globals.the_last = card_color.ToLower(); }

            //Check if I have the same color
            if (picked == true)
            {
                card_color = Globals.the_last;
                card_color = card_color.ToLower();
            }



            Globals.red = (double)item[0];
            Globals.blue = (double)item[1];
            Globals.green = (double)item[2];
            Globals.yellow = (double)item[3];



            //Who's playing next?
            // Json: Game --> Next_Player
            Globals.PlayerTurn = (int)rss["Game"]["Next_Player"];

            //Prendere la percentuale da togliere
            var steal_red = Globals.red * 100 * Globals.red;
            var steal_blue = Globals.blue * 100 * Globals.blue;
            var steal_green = Globals.green * 100 * Globals.green;
            var steal_yellow = Globals.yellow * 100 * Globals.yellow;

            if (picked == true)
            {
                if (card_color == "red") { steal_red = Globals.red / 100 / Globals.red; }
                if (card_color == "blue") { steal_blue = Globals.blue / 100 / Globals.blue; }
                if (card_color == "green") { steal_green = Globals.green / 100 / Globals.green; }
                if (card_color == "yellow") { steal_yellow = Globals.yellow / 100 / Globals.yellow; }

            }


            //Sommarla
            var sum_of_steal = 0.0;

            if (!picked)
            {
                if (card_color != "red")
                {
                    sum_of_steal = sum_of_steal + steal_red;
                    Globals.red = Globals.red - steal_red / 100;
                }
                if (card_color != "blue")
                {
                    sum_of_steal = sum_of_steal + steal_blue;
                    Globals.blue = Globals.blue - steal_blue / 100;
                }
                if (card_color != "green")
                {
                    sum_of_steal = sum_of_steal + steal_green;
                    Globals.green = Globals.green - steal_green / 100;
                }
                if (card_color != "yellow")
                {
                    sum_of_steal = sum_of_steal + steal_yellow;
                    Globals.yellow = Globals.yellow - steal_yellow / 100;
                }
            }
            else
            {
                var to_divide = 0;

                if (card_color == "red")
                {
                    //if (Globals.red != 0) { to_divide++; }
                    if (Globals.blue != 0) { to_divide++; }
                    if (Globals.green != 0) { to_divide++; }
                    if (Globals.yellow != 0) { to_divide++; }

                    var value = Globals.red / to_divide;
                    value = value - 0;
                    Globals.red = 0;
                    if (Globals.red != 0) { to_divide++; }
                    if (Globals.blue != 0) { Globals.blue = Globals.blue + value; }
                    if (Globals.green != 0) { Globals.green = Globals.green + value; }
                    if (Globals.yellow != 0) { Globals.yellow = Globals.yellow + value; }




                    //Globals.red = Globals.red + steal_red / 100;
                }
                if (card_color == "blue")
                {
                    if (Globals.red != 0) { to_divide++; }
                    //if (Globals.blue != 0) { to_divide++; }
                    if (Globals.green != 0) { to_divide++; }
                    if (Globals.yellow != 0) { to_divide++; }

                    var value = Globals.blue / to_divide;
                    value = value - 0;
                    Globals.blue = 0;
                    if (Globals.red != 0) { Globals.red = Globals.red + value; }
                    //if (Globals.blue != 0) { Globals.blue = Globals.blue + value; }
                    if (Globals.green != 0) { Globals.green = Globals.green + value; }
                    if (Globals.yellow != 0) { Globals.yellow = Globals.yellow + value; }

                }
                if (card_color == "green")
                {
                    if (Globals.red != 0) { to_divide++; }
                    if (Globals.blue != 0) { to_divide++; }
                    //if (Globals.green != 0) { to_divide++; }
                    if (Globals.yellow != 0) { to_divide++; }

                    var value = Globals.green / to_divide;
                    value = value - 0;

                    Globals.green = 0;
                    if (Globals.red != 0) { Globals.red = Globals.red + value; }
                    if (Globals.blue != 0) { Globals.blue = Globals.blue + value; }
                    //if (Globals.green != 0) { Globals.green = Globals.green + value; }
                    if (Globals.yellow != 0) { Globals.yellow = Globals.yellow + value; }

                }
                if (card_color == "yellow")
                {
                    if (Globals.red != 0) { to_divide++; }
                    if (Globals.blue != 0) { to_divide++; }
                    if (Globals.green != 0) { to_divide++; }
                    //if (Globals.yellow != 0) { to_divide++; }

                    var value = Globals.yellow / to_divide;
                    value = value - 0;
                    Globals.yellow = 0;
                    if (Globals.red != 0) { Globals.red = Globals.red + value; }
                    if (Globals.blue != 0) { Globals.blue = Globals.blue + value; }
                    if (Globals.green != 0) { Globals.green = Globals.green + value; }
                    //if (Globals.yellow != 0) { Globals.yellow = Globals.yellow + value; }

                }


            }




            //Assign new value to "main" color (increase %)
            if (card_color == "red")
            {
                Globals.red = Globals.red + sum_of_steal / 100;
            }
            if (card_color == "blue")
            {
                Globals.blue = Globals.blue + sum_of_steal / 100;
            }
            if (card_color == "green")
            {
                Globals.green = Globals.green + sum_of_steal / 100;
            }
            if (card_color == "yellow")
            {
                Globals.yellow = Globals.yellow + sum_of_steal / 100;
            }

            //Get top card color
            //Check who's playing (player turn)
            //Use clockwise ?

            //Color_Probability:  RED,BLUE,GREEN,YELLOW

            JObject game2 = rss["Game"]["Color_Probability"] as JObject;

            item = (JArray)game2[id_turn.ToString()];
            item[0].Replace(Globals.red);
            item[1].Replace(Globals.blue);
            item[2].Replace(Globals.green);
            item[3].Replace(Globals.yellow);



            Globals.JsonGame = rss.ToString();




            test.Text = Globals.JsonGame;
            string da_play = "(Blue)";

            double[] anArray = { Globals.red, Globals.blue, Globals.green, Globals.yellow };
            double max = anArray.Min();

            if (Globals.red == max) { da_play = "Red"; }
            if (Globals.blue == max) { da_play = "Blue"; }
            if (Globals.green == max) { da_play = "Green"; }
            if (Globals.yellow == max) { da_play = "Yellow"; }


            suggested_test.Text = "Suggested play: " + da_play;

            label15.Text = "Red: " + Math.Round(Globals.red * 100, 2) + "%     Blue: " + Math.Round(Globals.blue * 100, 2) + "%     Green: " + Math.Round(Globals.green * 100, 2) + "%     Yellow: " + Math.Round(Globals.yellow * 100, 2) + "%       Somma= " + (Globals.red + Globals.blue + Globals.green + Globals.yellow) * 100;

            if (Globals.red + Globals.blue + Globals.green + Globals.yellow == 0)
            {
                NewGame();

                suggested_test.Text = "Suggested play: (Blue)";
                Globals.red = 0.25;
                Globals.green = 0.25;
                Globals.blue = 0.25;
                Globals.yellow = 0.25;

                label15.Text = "Red: " + Globals.red * 100 + "%     Blue: " + Globals.blue * 100 + "%     Green: " + Globals.green * 100 + "%     Yellow: " + Globals.yellow * 100 + "%       Somma= " + (Globals.red + Globals.blue + Globals.green + Globals.yellow) * 100;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Globals.the_last = "red";
            CardProbability("", 1, true);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Globals.the_last = "blue";
            CardProbability("", 1, true);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Globals.the_last = "green";
            CardProbability("", 1, true);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Globals.the_last = "yellow";
            CardProbability("", 1, true);
        }
    }











    //shuffle Pile of cards
    //Thank you Matt ! https://stackoverflow.com/a/110570/13741865
    static class RandomExtensions
    {

        public static void Shuffle<T>(this Random rng, T[] array)
        {
            //if(DisableSeedGen)
            //Regex digitsOnly = new Regex(@"[^\d]");   
            //return digitsOnly.Replace(phone, "");
            //rng = new Random(0);


            int n = array.Length;
            while (n > 1)
            {
                int k = rng.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
    }
}
