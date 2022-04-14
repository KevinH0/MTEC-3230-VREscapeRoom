var speed : float = 1.0;

function Update () {
    transform.Rotate(Vector3.up * speed * Time.deltaTime);
}