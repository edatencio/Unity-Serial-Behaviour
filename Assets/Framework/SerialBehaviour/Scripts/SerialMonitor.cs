using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class SerialMonitor : MonoBehaviour
{
     /*************************************************************************************************
     *** Variables
     *************************************************************************************************/
     [SerializeField, BoxGroup("Settings")] private bool hideIfNotDebugBuild;
     [SerializeField, BoxGroup("Settings")] private GameObject canvas;
     [SerializeField, BoxGroup("Settings")] private Text text;
     [SerializeField, BoxGroup("Settings")] private Scrollbar verticalScrollbar;
     [SerializeField, BoxGroup("Settings")] private Button autoScrollButton;

     private bool autoScroll;
     private float vertical;

     // Unity's text only allows 65k vertices, each char should use 4 by default. However, if you add
     // shadows or outlines, it will use more.
     private const int TEXT_MAX_LENGTH = 10000;

     /*************************************************************************************************
     *** Start
     *************************************************************************************************/
     private void Start()
     {
          if (FindObjectOfType<SerialMonitor>() != this)
               Log.Error(name, "There's more than one SerialMonitor in scene.");

          Enabled = true;

          if (hideIfNotDebugBuild && !Debug.isDebugBuild)
               Disable();

          // Call to trigger an update of the button color
          AutoScrollOnClick();
     }

     /*************************************************************************************************
     *** Update
     *************************************************************************************************/
     private void Update()
     {
          if (Enabled)
          {
               if (Input.GetKeyDown(KeyCode.Keypad0))
                    text.text = "";

               if (Input.GetKeyDown(KeyCode.Keypad5))
                    AutoScrollOnClick();

               vertical = 0;
               if (Input.GetKey(KeyCode.Keypad8)) vertical = 1;
               if (Input.GetKey(KeyCode.Keypad2)) vertical = -1;

               if (vertical != 0)
               {
                    if (autoScroll)
                         AutoScrollOnClick();
                    verticalScrollbar.value += Time.deltaTime * vertical * 3f * verticalScrollbar.size;
               }
          }
     }

     /*************************************************************************************************
     *** Properties
     *************************************************************************************************/
     private bool Enabled { get; set; }

     /*************************************************************************************************
     *** Methods
     *************************************************************************************************/
     public void Add(params object[] message)
     {
          if (Enabled)
          {
               string msg = "";
               foreach (object s in message)
                    msg += s.ToString();

               bool textIsTooLong = (text.text.Length + msg.Length) > TEXT_MAX_LENGTH;

               if (textIsTooLong)
               {
                    int excess = (text.text.Length + msg.Length) - TEXT_MAX_LENGTH;
                    int index = text.text.IndexOf('\n', excess) + 1;
                    text.text = text.text.Remove(0, index);
               }

               text.text += string.Concat(msg, '\n');

               if (autoScroll)
                    verticalScrollbar.value = 0;
          }
     }

     public void Clear()
     {
          text.text = "";
     }

     public void AutoScrollOnClick()
     {
          if (Enabled)
          {
               autoScroll = !autoScroll;

               Image image = autoScrollButton.GetComponent<Image>();

               if (autoScroll)
                    image.color = new Color(0.196f, 1f, 0.196f, 1f);
               else
                    image.color = new Color(1f, 0.196f, 0.196f, 1f);
          }
     }

     private void Disable()
     {
          canvas.SetActive(false);
          Enabled = false;
     }
}
