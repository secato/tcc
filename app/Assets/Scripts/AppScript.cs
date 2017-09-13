using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



namespace Kudan.AR
{
	public class AppScript : MonoBehaviour
	{

		public Button btnRastrear;

		// variaveis
		public GameObject markerless;

		public GameObject KudanCamera;

		public KudanTracker _kudanTracker;
		// The tracker to be referenced in the inspector. This is the Kudan Camera object.
		public TrackingMethodMarkerless _markerlessTracking;
		// The reference to the markerless tracking method that lets the tracker know which method it is using


		void Start ()
		{
			if (StaticData.objeto == null) {
				Debug.Log ("Sem objeto");
				btnRastrear.enabled = false;
			} else {
				Debug.Log ("Com objeto");
				Instantiate (StaticData.objeto, new Vector3 (0, 0, 0), Quaternion.identity, markerless.transform);
			}
		}

		void Update () {
			if (Input.touchCount == 1) {
				Touch touch0 = Input.GetTouch (0);

				if (touch0.phase == TouchPhase.Moved) {
					//markerless.transform.Rotate (0f, touch0.deltaPosition.x, 0f);
					markerless.transform.GetChild(0).Rotate(0f, touch0.deltaPosition.x, 0f);
				}
			}

			if (Input.touchCount == 2) {
				Touch touchZero = Input.GetTouch (0);
				Touch touchOne = Input.GetTouch (1);

				// Find the position in the previous frame of each touch.
				Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
				Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

				// Find the magnitude of the vector (the distance) between the touches in each frame.
				float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
				float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

				// Find the difference in distances between each frame.
				float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
				if (deltaMagnitudeDiff > 1 || deltaMagnitudeDiff < -1) {
					deltaMagnitudeDiff -= 1;
					Vector3 newScale = markerless.transform.GetChild(0).localScale - new Vector3 (deltaMagnitudeDiff, deltaMagnitudeDiff, deltaMagnitudeDiff);
					markerless.transform.GetChild(0).localScale = newScale;
				}
			}
		}

		public void Rastrear ()
		{
			if (!_kudanTracker.ArbiTrackIsTracking ()) {
				// from the floor placer.
				Vector3 floorPosition;			// The current position in 3D space of the floor
				Quaternion floorOrientation;	// The current orientation of the floor in 3D space, relative to the device

				_kudanTracker.FloorPlaceGetPose (out floorPosition, out floorOrientation);	// Gets the position and orientation of the floor and assigns the referenced Vector3 and Quaternion those values
				_kudanTracker.ArbiTrackStart (floorPosition, floorOrientation);				// Starts markerless tracking based upon the given floor position and orientations

			} else {
				_kudanTracker.ArbiTrackStop ();
			}
		}

		public void DownloadModel ()
		{
			_kudanTracker.ArbiTrackStop ();
			_kudanTracker.StopTracking ();
			Initiate.Fade ("QRScan", Color.black, 1.5f);
		}

		public void ScreenShot ()
		{
			_kudanTracker.takeScreenshot ();

		}
	}
}
