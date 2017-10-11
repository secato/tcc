using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using QRCodeReaderAndGenerator;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;


public class DownloadModelScript : MonoBehaviour
{

	[SerializeField]
	RawImage rawImage; // utilizado para mostrar a captura da camera
	[SerializeField]
	Text txtResult; // mostra algumas mensagens, progresso do download, entre outros.

	GameObject previewObject; // variavel de apoio para instanciar uma prévia do objeto

	public Image downloadImage; // imagem da animacao de download
	public GameObject inputUrl; // campo de input da url

	IEnumerator Start ()
	{
		rawImage.enabled = false;
		downloadImage.enabled = false;
		inputUrl.SetActive (false);

		// pede autorizacao para uso da camera na primeira vez
		yield return Application.RequestUserAuthorization (UserAuthorization.WebCam);

		// se ja tinha sido baixado um objeto previamente faz a pré-visualização deste
		if (StaticData.objeto) {
			PreviewModel ();
		} 
	}

	void OnEnable ()
	{
		// adicionando handlers, isto é, os métodos que será chamados em caso de erro ou em caso de encontrar um qr code
		QRCodeManager.onError += HandleOnError;
		QRCodeManager.onQrCodeFound += HandleOnQRCodeFound;
	}

	void HandleOnQRCodeFound (ZXing.BarcodeFormat barCodeType, string barCodeValue)
	{
		rawImage.enabled = false; // desabilita a imagem que mostra a captura de video
		txtResult.text = barCodeValue; // mostra o resultado do que foi escaneado no QR code
		StartCoroutine (AsyncDownloadModel (barCodeValue)); // inicia uma rotina asíncrona para download do modelo 3D
	}

	void HandleOnError (string err)
	{
		Debug.LogError (err);
	}

	// método chamado pelo botão scan
	public void ScanQRCode ()
	{
		rawImage.enabled = true; // habilita a imagem que exibe a captura de video
		if (previewObject) { // se tinha algum objeto sendo pré visualizado este é destruído e limpa o campo de texto
			txtResult.text = "";
			Destroy (previewObject);
		}

		// configuracoes basicas da camera
		QRCodeManager.CameraSettings camSettings = new QRCodeManager.CameraSettings ();
		string rearCamName = GetRearCamName ();
		if (rearCamName != null) {
			camSettings.deviceName = rearCamName;
			camSettings.maintainAspectRatio = true;
			camSettings.scanType = ScanType.ONCE; // escanear um código de barras uam vez e para o escaneamento.
			QRCodeManager.Instance.ScanQRCode (camSettings, rawImage, 1f);
		}
		
	}

	// this function is require to call to stop scanning when camSettings.scanType = ScanType.CONTINUOUS;
	// no need to call when camSettings.scanType = ScanType.ONCE;
	public void StopScanning ()
	{
		QRCodeManager.Instance.StopScanning ();
	}

	string GetRearCamName ()
	{
		foreach (WebCamDevice device in WebCamTexture.devices) {
			if (!device.isFrontFacing) {
				
				return device.name;
			}
		}
		return null;
	}

	// rotina de download asícrona
	IEnumerator AsyncDownloadModel (string url)
	{
		Debug.Log (url);
		// habilita a imagem animada de download
		downloadImage.enabled = true; 
		// faz uma requisicao web para determinada url
		UnityWebRequest request = UnityWebRequest.GetAssetBundle (url, 0); 
		request.Send ();

		txtResult.text = "Downloading...";
		while (!request.isDone) {
			// exibe o resultado o progresso do download
			txtResult.text = Mathf.Round (request.downloadProgress * 100).ToString () + "%";
			yield return null;
		}

		// se acontecer algum erro
		if (request.isError) {
			Debug.Log (request.error);
			txtResult.text = "Não foi possível obter o modelo 3D";
			downloadImage.enabled = false;
		} else {
			// carrega o assetbundle
			AssetBundle bundle = DownloadHandlerAssetBundle.GetContent (request);

			// se carregado corretamente
			if (bundle) {
				txtResult.text = "Download concluído...";
				// obtem o objeto de nome model do asset bundle
				StaticData.objeto = bundle.LoadAsset<GameObject> ("model"); 
				downloadImage.enabled = false;

				// se já estava pré visualizando um objeto
				if (previewObject) 
					// destroi o objento
					Destroy (previewObject);
				// chama o método responsável pela pré-visualização
				PreviewModel (); 
				//evita um erro ao tentar importar o mesmo asset bundle mais de uma vez
				bundle.Unload (false); 
			} else {
				txtResult.text = "Não foi possível obter o modelo 3D";	
			}
		}
	}

	//método de pré-visualização do modelo baixado
	void PreviewModel ()
	{
		txtResult.text = "Pré-Visualização";
		rawImage.enabled = false;
		previewObject = StaticData.objeto;

		// o segundo parâmetro passa a posição inicial do objeto, -100 em Y é para ele ficar um pouco mais alto
		previewObject = Instantiate (previewObject, new Vector3 (0, -100, 250), previewObject.transform.rotation);

	}

	// volta para a cena de RA
	public void VoltarPrincipal ()
	{
		Initiate.Fade ("RA", Color.black, 1.5f);
	}

	public void inputVisible ()
	{
		if (inputUrl.activeSelf)
			inputUrl.SetActive (false);
		else
			inputUrl.SetActive (true);
	}

	// ao clicar no botão OK do input
	public void urlDownload (string url)
	{
		inputUrl.SetActive (false);
		if (url != "")
			StartCoroutine (AsyncDownloadModel (url));
	}


	// update é chamado a cada frame
	void Update ()
	{

		// se tiver algum objeto sendo pré visualizado
		if (previewObject) {

			// um dedo utilizado no touch, rotacao
			if (Input.touchCount == 1) {
				Touch touch0 = Input.GetTouch (0);

				if (touch0.phase == TouchPhase.Moved) {
					previewObject.transform.Rotate (0f, touch0.deltaPosition.x*0.75f, 0f);
				}
			}

			// dois dedos utilizado no touch, movimento de pinça, escala
			if (Input.touchCount == 2) {

				Touch touchZero = Input.GetTouch (0); // primeiro toque
				Touch touchOne = Input.GetTouch (1); // segundo toque

				// obtem posicao anterior conforme move o dedo
				Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
				Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

				// obten a distancia dos dedos a cada frame
				float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
				float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

				// obtem a diferença das distancia e realiza a escala
				float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
				if (deltaMagnitudeDiff > 1.5f || deltaMagnitudeDiff < -1.5f) {
					deltaMagnitudeDiff -= 1.5f;
					Vector3 newScale = previewObject.transform.localScale - new Vector3 (deltaMagnitudeDiff, deltaMagnitudeDiff, deltaMagnitudeDiff);

					// verificacao para evitar escalar o objeto negativamente
					if (newScale.x >= 10)
						previewObject.transform.localScale = newScale;
				}
			}
		}
	}

}
