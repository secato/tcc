using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



namespace Kudan.AR
{
	public class RaScript : MonoBehaviour
	{

		// variaveis interface
		public Button btnRastrear;
		public Text txtAltura;
		public Slider sliderAltura;

		// variaveis ra
		public GameObject markerless; //o objeto que sera rastreado é filho deste objeto

		// variaveis kudan
		public KudanTracker _kudanTracker; // The tracker to be referenced in the inspector. This is the Kudan Camera object.
		public TrackingMethodMarkerless _markerlessTracking; // The reference to the markerless tracking method that lets the tracker know which method it is using

		void Start ()
		{

			// mostra a altura inicial no txtAltura
			txtAltura.text = (sliderAltura.value * 100).ToString();

			// se não existir um objeto na classe estática desabilita o botao rastrear, 
			if (StaticData.objeto == null) {
				btnRastrear.enabled = false;
			} else {
				// se existir instancia este como filho de markerless, isso é só para instanciar o objeto, ele só ficara visível quando for clicado no ícone de "olho"
				Instantiate (StaticData.objeto, new Vector3 (0, 0, 0), StaticData.objeto.transform.rotation, markerless.transform);
			}
		}

		void Update () {

			// se estiver rastreando (mostrando alguma coisa) permite rotacionar o objeto
			if (_kudanTracker.ArbiTrackIsTracking ()) {
				if (Input.touchCount == 1) {
					Touch touch0 = Input.GetTouch (0);

					if (touch0.phase == TouchPhase.Moved) {
						//markerless.transform.Rotate (0f, touch0.deltaPosition.x, 0f);
						markerless.transform.GetChild (0).Rotate (0f, touch0.deltaPosition.x*0.75f, 0f);
					}
				}

				// permite escalar o objeto
//				if (Input.touchCount == 2) {
//					Touch touchZero = Input.GetTouch (0);
//					Touch touchOne = Input.GetTouch (1);
//
//					// Find the position in the previous frame of each touch.
//					Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
//					Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
//
//					// Find the magnitude of the vector (the distance) between the touches in each frame.
//					float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
//					float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
//
//					// Find the difference in distances between each frame.
//					float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
//					if (deltaMagnitudeDiff > 1.5f || deltaMagnitudeDiff < -1.5f) {
//						deltaMagnitudeDiff -= 1.5f;
//						Vector3 newScale = markerless.transform.GetChild (0).localScale - new Vector3 (deltaMagnitudeDiff, deltaMagnitudeDiff, deltaMagnitudeDiff);
//						if (newScale.x >= 10)
//							markerless.transform.GetChild (0).localScale = newScale;
//						//markerless.transform.GetChild(0).localScale = newScale;
//					}
//				}
			}
		}

		// metodo chamado pelo slider de altura, seta a altura do "piso virtual" do kudan, só funciona com o rastreamento parado
		public void Altura(float altura){

			// se estiver rastreado para o rastreio
			if (_kudanTracker.ArbiTrackIsTracking ()) {
				_kudanTracker.ArbiTrackStop ();
			}
			// seta essa altura
			_kudanTracker.SetArbiTrackFloorHeight (altura * 100);

			// exibe no txtAltura 
			txtAltura.text = Mathf.Round (altura * 100).ToString();

			//Debug.Log (altura * 100);
		}

		// metodo chamado pelo icone "olho", inicia ou para o rastreamento dependendo do estado
		public void Rastrear ()
		{
			if (!_kudanTracker.ArbiTrackIsTracking ()) {
				// baseado no icone de posicionamento (Arrow)
				Vector3 floorPosition;			
				Quaternion floorOrientation;

				// obtem o posicionamento e orientacao do "piso" da cena
				_kudanTracker.FloorPlaceGetPose (out floorPosition, out floorOrientation);
				// inicia o rastreamento sem marcador baseado nos parametros de posicao e orientacao
				_kudanTracker.ArbiTrackStart (floorPosition, floorOrientation);				
			} else {
				_kudanTracker.ArbiTrackStop ();
			}
		}

		// icone de download, chama a cena de download dos modelos
		public void DownloadModel ()
		{
			_kudanTracker.ArbiTrackStop ();
			_kudanTracker.StopTracking ();
			Initiate.Fade ("DownloadModel", Color.black, 1.5f);
		}

		// icone de screenshot, some com a interface e captura a tela
		public void ScreenShot ()
		{
			_kudanTracker.takeScreenshot ();

		}
	}
}
