using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Alteruna;
using System.Collections;

public class RoomMenu : MonoBehaviour
{
    private const float REFRESH_LIST_INTERVAL = 5.0f;

    [SerializeField]
    private Text TitleText;
    [SerializeField]
    private ScrollRect ScrollRect;
    [SerializeField]
    private GameObject LANEntryPrefab;
    [SerializeField]
    private GameObject WANEntryPrefab;
    [SerializeField]
    private GameObject CloudImage;
    [SerializeField]
    private GameObject ContentContainer;
    [SerializeField]
    private Button StartButton;
    [SerializeField]
    private Button LeaveButton;

    public bool AutomaticallyRefresh = true;
    public float RefreshInterval = 5.0f;

    private Multiplayer _aump;
    private List<Room> _rooms = new List<Room>();
    private List<GameObject> _roomObjects = new List<GameObject>();

    private void Connected(Multiplayer multiplayer, Endpoint endpoint)
    {
        if (TitleText != null)
        {
            TitleText.text = "Rooms";
        }

        if (isActiveAndEnabled)
        {
            StartCoroutine(nameof(RefreshRooms));
        }
    }

    private void Disconnected(Multiplayer multiplayer, Endpoint endPoint)
    {
        if (TitleText != null)
        {
            TitleText.text = "Reconnecting..";
        }
    }

    private void UpdateList(Multiplayer multiplayer)
    {
        for (int i = 0; i < _roomObjects.Count; i++)
        {
            Destroy(_roomObjects[i]);
        }
        _roomObjects.Clear();

        if (ContentContainer != null)
        {
            for (int i = 0; i < multiplayer.AvailableRooms.Count; i++)
            {
                Room room = multiplayer.AvailableRooms[i];

                GameObject entry;
                if (room.Local)
                {
                    entry = Instantiate(WANEntryPrefab, ContentContainer.transform);
                }
                else
                {
                    entry = Instantiate(LANEntryPrefab, ContentContainer.transform);
                }

                entry.SetActive(true);
                _roomObjects.Add(entry);

                entry.GetComponentInChildren<Text>().text = room.Name;
                entry.GetComponentInChildren<Button>().onClick.AddListener(() => { room.Join(); });
            }
        }
    }

    private void JoinedRoom(Multiplayer multiplayer, Room room, User user)
    {
        if (TitleText != null)
        {
            TitleText.text = "In Room " + room.Name;
        }
    }

    private void LeftRoom(Multiplayer multiplayer)
    {
        if (TitleText != null)
        {
            TitleText.text = "Rooms";
        }
    }

    private void Start()
    {
        if (_aump == null)
        {
            _aump = FindObjectOfType<Multiplayer>();
            if (!_aump)
            {
                Debug.LogError("Unable to find a active object of type Multiplayer.");
            }
        }

        if (_aump != null)
        {
            _aump.Connected.AddListener(Connected);
            _aump.Disconnected.AddListener(Disconnected);
            _aump.RoomListUpdated.AddListener(UpdateList);
            _aump.RoomJoined.AddListener(JoinedRoom);
            _aump.RoomLeft.AddListener(LeftRoom);
            StartButton.onClick.AddListener(() => { _aump.JoinOnDemandRoom(); });
            LeaveButton.onClick.AddListener(() => { _aump.CurrentRoom?.Leave(); });
        }

        if (TitleText != null)
        {
            TitleText.text = "Connecting..";
        }
    }

    private IEnumerator RefreshRooms()
    {
        while (AutomaticallyRefresh) {
            yield return new WaitForSeconds(RefreshInterval);
            _aump.RefreshRoomList();
        }
    }
}
