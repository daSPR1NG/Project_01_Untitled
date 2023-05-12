using UnityEngine;
using System.Collections;
using dnSR_Coding.Utilities;
using NaughtyAttributes;
using System.Collections.Generic;

namespace dnSR_Coding
{
    public static class EventManager
    {
        //  Subject => change et envoie la notification de changement
        //  Observer => attend un changement et ex�cute qlq chose en fonction

        //  Utiliser un ID pour diff�rencier les objets -> instance ID
        //  Stocker les events ici, les observers pourront subscribe depuis cet objet static

        //  Comment coupler les events d'un subject � cet objet ? => les stocker directement ici,
        //  et les appelants le feront en passant par cette classe
    }
}