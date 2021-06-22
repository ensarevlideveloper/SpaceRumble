using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SpaceRumble.Player;
namespace SpaceRumble.UI
{
    public class PlayerUI : MonoBehaviour
    {
        public Image Healthbar;
        public Text Scoreboard;


        private List<PlayerManager> players = new List<PlayerManager>();

        public Dictionary<string, int> scoreTable = new Dictionary<string, int>();
        int maxPlayersOnScoreboard = 10;
        int maxPlayerNameLength = 12;



        public void Start()
        {
            //scoreTable = new Dictionary<string, int>();
            this.Reset();
            Healthbar.fillAmount = 1f;

        }

        public void Reset()
        {
            Scoreboard.text = "";

            for (int i = 0; i < maxPlayersOnScoreboard; i++)
            {
                Scoreboard.text += (i + 1) + "." + "\n";
            }
        }

        public void setHealth(int health)
        {
            Debug.Log("Set Health to " + (health) / 100f);
            Healthbar.fillAmount = (float)health / 100f;
        }

        public void setScore(int score, string ID)
        {
            Debug.Log("updating scoretable" + ID);

            scoreTable[ID] = score;
            updateScoreTableGUI();
        }

        public void updateScoreTableGUI()
        {
            this.Reset();
            //Scoreboard.text = "";
            Debug.Log("updating scoretable 2");
            ArrayList topPlayers = new ArrayList();
            //string[] topPlayers = new string[maxPlayersOnScoreboard];
            for (int i = 0; i < maxPlayersOnScoreboard; i++)
            {
                topPlayers.Insert(i, "");
            }

            ArrayList topScores = new ArrayList();
            for (int i = 0; i < maxPlayersOnScoreboard; i++)
            {
                topScores.Insert(i, 0);
            }

            string scoreboardText = "";
            IDictionaryEnumerator scoreTableEnumerator = scoreTable.GetEnumerator();

            Debug.Log(topScores.Count + "," + topPlayers.Count);
            while (scoreTableEnumerator.MoveNext())
            {
                bool isPlaced = false;
                Debug.Log("Before for");
                for (int i = 0; i < maxPlayersOnScoreboard; i++)
                {

                    if (isPlaced == false)
                    {
                        int value = int.Parse(scoreTableEnumerator.Value.ToString());
                        Debug.Log(topScores[i] + "," + value);
                        if ((int)topScores[i] <= value)
                        {
                            Debug.Log("placed");
                            string key = scoreTableEnumerator.Key.ToString();
                            topPlayers.Insert(i, key);
                            topScores.Insert(i, value);
                            topPlayers.RemoveAt(topPlayers.Count - 1);
                            topScores.RemoveAt(topScores.Count - 1);
                            isPlaced = true;
                        }
                    }
                }
                Debug.Log("After for");
            }

            Debug.Log("Before second for");
            Debug.Log(topScores.Capacity + "," + topPlayers.Capacity);
            for (int i = 0; i < maxPlayersOnScoreboard; i++)
            {
                if (topPlayers[i].ToString().Length > maxPlayerNameLength)
                {
                    topPlayers[i] = topPlayers[i].ToString().Substring(0, maxPlayerNameLength - 5);
                }
                scoreboardText += (i + 1) + "." + topPlayers[i] + "\n";
                Debug.Log(scoreboardText);
                Scoreboard.text = scoreboardText;
            }
            Debug.Log("After second for");

            Debug.Log(scoreboardText);
            Scoreboard.text = scoreboardText;
        }

        public void updateScoretable () {
            players.Sort((a, b) => b.GetScore().CompareTo(a.GetScore()));
            PlayerManager[] topPlayers = players.ToArray();
            string scoreboardText = "";

            int max = 0;
            if (topPlayers.Length > maxPlayersOnScoreboard) {
                max = maxPlayersOnScoreboard;
            }
            else {
                max = topPlayers.Length;
            }
            for (int i = 0; i < max; i++)
            {
                if (topPlayers[i] != null)
                if (topPlayers[i].GetUsername().Length > maxPlayerNameLength)
                {
                    scoreboardText +=  topPlayers[i].GetUsername().Substring(0, maxPlayerNameLength - 5);
                }
                else
                {
                    scoreboardText += (i + 1) + "." + topPlayers[i].GetUsername() + "\n";
                }
                Debug.Log(scoreboardText);
                Scoreboard.text = scoreboardText;
            }
            Debug.Log("After second for");

            Debug.Log(scoreboardText);
            Scoreboard.text = scoreboardText;
        }
        

        public void addPlayer(PlayerManager p)
        {
            if (players.Contains(p) == true)
            {
                //Nothing
            }
            else
            {
                this.players.Add(p);
            }
        }

        public void removePlayer (PlayerManager p) {
            this.players.Remove(p);
        }



    }





}





