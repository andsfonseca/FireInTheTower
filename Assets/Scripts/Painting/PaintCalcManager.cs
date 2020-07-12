using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaintTower.Scripts;
using PaintTower.Enum;

namespace PaintTower.PaintCalc {
    public class PaintCalcManager : MonoBehaviour {
        public PaintCalculation[] objectsToPaint { private get; set; }
        [HideInInspector] public float redPercent = 0;
        [HideInInspector] public float yellowPercent = 0;
        [HideInInspector] public float greenPercent = 0;
        [HideInInspector] public float bluePercent = 0;
        private int frames = 0;
        // Start is called before the first frame update
        void Start() {
            //myScriptObjects = GameObject.FindObjectsOfType(typeof(PaintCalculation)) as PaintCalculation[]; 
        }

        // Update is called once per frame
        void Update() {
            /*
            if (GameLogic.Instance.CurrentGameState == GameState.PLAY) {
                frames++;
                if (frames == 30) {
                    calcPercent();
                    frames = 0;
                }
            }
            */
        }
        void calcPercent() {
            float totalRedCount = 0;
            float totalYellowCount = 0;
            float totalGreenCount = 0;
            float totalBlueCount = 0;
            foreach (PaintCalculation towerPiece in objectsToPaint) {
                int[] temp;
                temp = towerPiece.calcPaint();
                totalRedCount += temp[0];
                totalYellowCount += temp[1];
                totalGreenCount += temp[2];
                totalBlueCount += temp[3];
            }
            float total = 0;
            total = totalRedCount + totalYellowCount + totalGreenCount + totalBlueCount;
            if (total != 0) {
                if (totalRedCount != 0) {
                    redPercent = totalRedCount / total;
                }
                if (totalYellowCount != 0) {
                    yellowPercent = totalYellowCount / total;
                }
                if (totalGreenCount != 0) {
                    greenPercent = totalGreenCount / total;
                }
                if (totalBlueCount != 0) {
                    bluePercent = totalBlueCount / total;
                }
            }
            //Debug.Log("BIG PERCENT DEBUG ==> RED: " + redPercent + " YELLOW: " + yellowPercent + " GREEN: " + greenPercent + " BLUE: " + bluePercent + " TOTAL: " + total);
            //Debug.Log("BUG COUNT ==> RED: " + totalRedCount + " YELLOW: " + totalYellowCount + " GREEN: " + totalGreenCount + " BLUE: " + totalBlueCount + " TOTAL: " + total); 
        }
    }
}