using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoinCount : MonoBehaviour
{
    public int coinCount = 0;
    public TextMeshProUGUI coinText;
    //public AudioSource coinAudioSource;

    // Update is called once per frame
    void Update()
    {
        //coinAudioSource.Play();
        coinText.text = coinCount.ToString();
    }
}
