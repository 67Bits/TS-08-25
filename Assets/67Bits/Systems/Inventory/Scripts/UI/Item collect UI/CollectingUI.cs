using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectingUI : Singleton<CollectingUI>
{
    [SerializeField] private List<ICollectorUIElement> _collectorsList = new();
    [SerializeField] private Dictionary<ItemID, ICollectorUIElement> _collectorsDict = new();
    [SerializeField] private Image _flyingItemPrefab;
    [SerializeField] private float _spritesSpeed = 1000f;
    [SerializeField] private float _repeatInterval = 0.05f;
    [SerializeField] private float _randomInitPosVariation = 80f;
    private Camera _camera;
    private List<Image> _imagePool = new ();

    private void Start()
    {
        _camera = Camera.main;
    }

    #region Collectors
    public ICollectorUIElement GetCollector(ItemID item)
    {
        if (_collectorsDict.ContainsKey(item))
            return _collectorsDict[item];

        return null;
    }

    public bool SubscribeToCollectors(ItemID item, ICollectorUIElement counter)
    {
        if (_collectorsDict.ContainsKey(item))
            return false;

        _collectorsDict.Add(item, counter);
        _collectorsList.Add(counter);
        return true;
    }

    public void UnsubscribeAnyFromCollectors(ItemID item)
    {
        if (_collectorsDict.ContainsKey(item))
        {
            _collectorsList.Remove(GetCollector(item));
            _collectorsDict.Remove(item);
        }
    }

    public void UnsubscribeFromCollectors(ItemID item, ICollectorUIElement inventoryCounterUI)
    {
        if (_collectorsDict.ContainsKey(item) && _collectorsDict[item] == inventoryCounterUI)
        {
            _collectorsList.Remove(GetCollector(item));
            _collectorsDict.Remove(item);
        }
    }
    #endregion

    #region Collect item
    public bool TryCollectAnimation(Amount<ItemID> amount, Vector2 initPosition)
    {
        return TryCollectAnimation(amount.Unit, amount.Quantity, initPosition);
    }

    public bool TryCollectAnimation(ItemID itemID, int quantity, Vector2 initPosition)
    {
        if (quantity < 0)
            return false;

        if (!_collectorsDict.ContainsKey(itemID))
            return false;

        StartCollectAnimation(itemID, quantity, initPosition);

        return true;
    }

    private void StartCollectAnimation(ItemID itemID, int quantity, Vector2 initPosition)
    {
        int spritesQuantity = GetSpritesQuantity(quantity);
        var collector = _collectorsDict[itemID];
        TrackedPosition finalPosition = collector.CollectingUIPosition();
        float movementDuration = Vector2.Distance(initPosition, finalPosition.Position) / _spritesSpeed;

        collector.OnCollectAnimationStart();
        collector.HoldQuantityForAnimation(quantity, movementDuration);
        StartCoroutine(RepeatCollectAnimationRoutine(itemID, initPosition, finalPosition, movementDuration, spritesQuantity, _repeatInterval));
    }
    #endregion

    #region Animation data
    private int GetSpritesQuantity(int quantity)
    {
        // 1 item -> 1 sprite
        // 10 items -> 10 sprite
        // 100 items -> 20 sprite
        // 1000 items -> 30 sprite

        int spritesQuantity = 0;

        if (quantity > 100)
        {
            spritesQuantity += (quantity / 100);
            quantity = 99;
        }

        if (quantity > 10)
        {
            spritesQuantity += (quantity / 10);
            quantity = 9;
        }

        spritesQuantity += quantity;
        return spritesQuantity;
    }
    #endregion

    #region Images pool
    private Image GetImageFromPoolOrNew()
    {
        var size = _imagePool.Count;

        for (int i = 0; i < size; i++)
        {
            if (_imagePool[i] != null)
            {
                Image selectedImage = _imagePool[i];
                _imagePool.RemoveAt(i);
                return selectedImage;
            }
        }

        GameObject copy = Instantiate(_flyingItemPrefab.gameObject, this.transform);
        if (copy.TryGetComponent(out Image copyImage))
        {
            return copyImage;
        }

        Debug.LogError("Couldn' find an image.\n\n", this);
        return null;
    }

    private void SendImageToPool(Image selectedImage)
    {
        selectedImage.gameObject.SetActive(false);
        _imagePool.Add(selectedImage);
    }
    #endregion

    #region Animation routine
    private IEnumerator RepeatCollectAnimationRoutine(ItemID item, Vector2 initUiPosition, TrackedPosition finalUiPosition, float movementDuration, int RepeatQuantity, float repeatInterval)
    {
        var waitFrame = new WaitForEndOfFrame();
        var waitInterval = new WaitForSeconds(repeatInterval);

        for (int i = 0; i < RepeatQuantity; i++)
        {
            var initPosVariation = _randomInitPosVariation * new Vector2(UnityEngine.Random.value - 0.5f, UnityEngine.Random.value - 0.5f);
            StartCoroutine(CollectAnimationRoutine(item, initUiPosition + initPosVariation, finalUiPosition, movementDuration));
            yield return waitInterval;
            yield return waitFrame;
        }
    }

    private IEnumerator CollectAnimationRoutine(ItemID item, Vector2 initUiPosition, TrackedPosition finalUiPosition, float movementDuration)
    {
        Image copyImage = GetImageFromPoolOrNew();
        if (copyImage == null)
            yield break;
        
        copyImage.sprite = InventoryService.Instance.GetItemUIData(item).Sprite;
        copyImage.rectTransform.position = initUiPosition;
        copyImage.gameObject.SetActive(true);

        var waitFrame = new WaitForEndOfFrame();
        yield return waitFrame;

        // Moving
        float timeRemaining = movementDuration;
        float movePercentage = 0.0f;
        while (timeRemaining > 0.0f)
        {
            yield return waitFrame;
            movePercentage = Time.deltaTime / timeRemaining;
            timeRemaining -= Time.deltaTime;

            var aimedMovement = (finalUiPosition.Position - copyImage.rectTransform.position);
            copyImage.rectTransform.position += aimedMovement * movePercentage;
        }

        // When got in place
        copyImage.rectTransform.position = finalUiPosition.Position;
        SendImageToPool(copyImage);
        yield return waitFrame;

        _collectorsDict[item].OnCollect();
    }
    #endregion
}


/// <summary> Helps animatin handle different objects as position. </summary>
public class TrackedPosition
{
    private Transform _transform;
    private Vector3 _position;
    public Vector3 Position { get { return GetPosition(); } }

    public TrackedPosition(Transform transform)
    {
        _transform = transform;
        _position = _transform.position;
    }

    public TrackedPosition(Vector3 position)
    {
        _position = position;
    }

    public Vector3 GetPosition()
    {
        if (_transform != null)
        {
            _position = _transform.position;
            return _transform.position;
        }

        return _position;
    }
}