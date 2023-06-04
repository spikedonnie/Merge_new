using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SolClovser.EasyMultipleHealthbar
{
    public class HealthbarController : MonoBehaviour
    {
        [Tooltip("Drag and drop the slider used for health bar.")]
        public Slider healthBarSlider;

        [Tooltip("Drag and drop the slider used for health bar.")]
        public RectTransform healthBarSliderRectTransform;

        private EasyMultipleHealthbar _multipleHealthbarManager;
        private Transform _target;
        private Vector3 _positionOffset;
        private Camera _calculationCamera;

        #region Mono Methods

        private void Awake()
        {
            _multipleHealthbarManager = EasyMultipleHealthbar.Instance;
            
            // Cache the camera component on awake
            _calculationCamera = Camera.main;
        }

        private void LateUpdate()
        {
            if (_target) Reposition();
        }

        #endregion

        #region Private Methods

        // Update slider position so it will stay at the head of our target
        private void Reposition()
        {
            if (!_multipleHealthbarManager.containerCanvas)
            {
                Debug.LogWarning("Container canvas is not set. Please see the setup guide.");
                return;
            }

            // Get container canvas from manager instance
            RectTransform containerCanvas = _multipleHealthbarManager.containerCanvas;

            // Get our target position and add offset setting to it
            Vector3 targetPosition = _target.position + _positionOffset;

            // Convert world position to view port position
            Vector2 viewPortPosition = _calculationCamera.WorldToViewportPoint(targetPosition);

            // Calculate screen position
            Vector2 screenPosition = new Vector2(
                ((viewPortPosition.x * containerCanvas.sizeDelta.x) - (containerCanvas.sizeDelta.x * 0.50f)),
                ((viewPortPosition.y * containerCanvas.sizeDelta.y) - (containerCanvas.sizeDelta.y * 0.50f)));

            // Set our slider position
            healthBarSliderRectTransform.anchoredPosition = screenPosition;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Setup the healthbar after retrieving it for the first time
        /// </summary>
        /// <param name="maxValue"></param>
        /// <param name="currentValue"></param>
        /// <param name="target"></param>
        /// <param name="positionOffset"></param>
        public void SetupUI(float maxValue, float currentValue, Transform target, Vector3 positionOffset)
        {
            _target = target;
            _positionOffset = positionOffset;

            healthBarSlider.maxValue = maxValue;
            healthBarSlider.value = currentValue;

            gameObject.SetActive(true);
        }

        /// <summary>
        /// Update healthbar fill
        /// </summary>
        /// <param name="currentValue"></param>
        public void UpdateUI(float currentValue)
        {
            healthBarSlider.value = currentValue;
        }

        /// <summary>
        /// Return healthbar to pool
        /// </summary>
        public void Return()
        {
            transform.SetParent(_multipleHealthbarManager.SortingLayers[0]);
            _multipleHealthbarManager.ReturnHealthbar(this);
        }

        /// <summary>
        /// Move healthbar to top in hierarchy so it will appear under other healthbars when overlapping happens
        /// </summary>
        public void MoveToTopInLayerHierarchy()
        {
            transform.SetAsFirstSibling();
        }
        
        /// <summary>
        /// Move healthbar to bottom in hierarchy so it will appear over other healthbars when overlapping happens
        /// </summary>
        public void MoveToBottomInLayerHierarchy()
        {
            transform.SetAsLastSibling();
        }

        /// <summary>
        /// Set the healthbar order in the layer hierarchy manually so you can adjust which healthbar should be on top when overlapping happens
        /// </summary>
        /// <param name="index"></param>
        public void MoveToIndexInLayerHierarchy(int index)
        {
            transform.SetSiblingIndex(index);
        }

        /// <summary>
        /// Set the sorting layer of the healthbar so it can stay on top of the other healthbars at all times when overlapping happens.
        /// You might need to increase the layer count on Easy Multiple Healthbar object beforehand
        /// </summary>
        /// <param name="index"></param>
        public void SetSortingLayer(int index)
        {
            if (index >= _multipleHealthbarManager.SortingLayers.Length)
            {
                Debug.LogWarning("Layer not found! Please setup the layers on Easy Multiple Healthbar object.");
                return;
            }
            
            transform.SetParent(_multipleHealthbarManager.SortingLayers[index], true);
        }
        #endregion
    }
}