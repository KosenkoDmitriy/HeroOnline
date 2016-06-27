var minIntensity : float = 1.0;
var maxIntensity : float = 2.0;
var variationSpeed : float = 1.0;
var enableVariation : boolean = true;

private var increase : boolean = true;
private var decrease : boolean;
 
function Update () {
	if (increase == true)
		decrease = false;
	else
		decrease = true;

	if (enableVariation == true){
		if (increase == true)
			light.intensity += variationSpeed*Time.deltaTime;
		if (decrease == true)
			light.intensity -= variationSpeed*Time.deltaTime;
	}
	
	if (light.intensity >= maxIntensity)
		increase = false;
	if (light.intensity <= minIntensity)
		increase = true;
}