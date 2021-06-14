# AntColonySimulator

Realizado por: Nicolás Pastore Burgos

# Descripción del proyecto

Este proyecto demuestra el funcionamiento de una colonia de hormigas muy simplificadamente. Comenzará con unas pocas hormigas que buscarán comida por un mapa de manera aleatoria partiendo desde su hormiguero. De ahí, siguiendo un modelo que asemeja la realidad, estas irán dejando un rastro de feromonas para poder regresar al hormiguero, o guiar a otras hormigas a encontrar la comida que han encontrado. A medida que estas hormigas vayan consiguiendo introducir comida en el hormiguero, más hormigas podrán ir naciendo. Además si encuentran peligros, dejarán unas feromonas que hará que el resto de hormigas eviten ese lugar. Al encontrar esta comida, intentaran volver a su hormiguero a depositar lo encontrado y permitir que más hormigas nazcan.

# Descripción de la escena

La escena representa un modelo de terrario horizontal, teniendo unos bordes marcados y obstáculos que se podrán ir expandiendo.

## Hormigas

Las hormigas están representadas como pequeños marcas negras. Estas irán dejando un rastro de feromonas de distintos colores en función de su estado actual. Además, al seleccionarlas en el editor de Unity, se podrá ver sus detectores lógicos y físicos que utilizan para desplazarse por el entorno.

## Feromonas

Estas se ven como rectángulos de colores, en función del estado de la hormiga:
- Idle: Si la hormiga esta en el estado normal, este será un rastro gris que irá desapareciendo con el tiempo.
- Food: Si la hormiga tiene comida, está dejará un rastro azul que con el tiempo irá desapareciendo y volviéndose amarillo.
- Danger: Si la hormiga peligra, por haberse quedado atascada o por haberse encontrado con un elemento peligroso, dejará un rastro rojo que irán desapareciendo y volviéndose cían.

## Hormiguero

El hormiguero se ve como un agujero en el suelo. Al seleccionarlo en el editor de Unity se puede ver el radio en que las hormigas podrán nacer como una esfera verde.

## Comida

La comida se ve como melocotones desperdigadas por el mapa.

## Puntos de peligro

Se ven como cuadrados de lava.

## Muros

Son rectángulos amarillos, que además de rodear todo el recinto, podrán encontrarse rotados por el mapa.

## Interfaz

La interfaz mostrará muchos datos sobre la propia simulación y algunos slider y botones para poder alterar su funcionamiento. Arriba a la izquierda podremos ver datos básicos de la prueba: el número de hormigas, comidas en el mapa y en el hormiguero, el número de lugares peligrosos en el mapa y el número de feromonas activas. Bajando podremos ver dos slider y un toggle. Este primero sirve para activar el limite superior de hormigas y los dos sliders sirven para modificar el número máximo del slider y el coste de producir las hormigas. En la seccióno intermedia vemos 4 botones: los dos superiores para reiniciar la prueba y apagar la aplicación y los dos inferiores para añadir comida al hormiguero o spawnear una hormiga respetando el limite máximo. Por último a la derecha están los controles del jugador, y los números de las hormigas en sus distintos estados posibles: Idle, WithFood y Danger.

## Controles del jugador

El jugador podrá clicar y spawnear, solo sobre el mapa, comida o zonas de peligro clicando con el click izquierdo o el derecho del ratón respectivamente.

# Descripción de la IA

Toda la inteligencia artificial esta hecha con código de C#, sin el uso de Bolt ni árboles de comportamientos. Tampoco se utiliza navegación de Unity, ya que rompería el propósito de la practica.

## Hormigas

La inteligencia artificial de las hormigas es donde más trabajo hay en el proyecto. El comportamiento depende bastante de en que estado en concreto encuentre cada hormiga, pero por lo general, todas deambularán con un rumbo aleatorio (con excepciones), y ese rumbo estará muy influenciado por lo que son capaces de ver y las feromonas que pueden oler delante, a su izquierda y derecha. Estos detectores son muy diferentes entre sí. Para detectar el hormiguero, la comida y los puntos de peligro la hormiga utiliza un cono de visión donde en función de la distancia y del ángulo de visión serán capaces de percibirlos o no. La detección de muros es parecida, pero en vez de un cono de visión se lanzan 3 raycasts: uno delante y los otros dos con un ángulo del primero. Por ultimo la detección de feromonas se realiza con tres puntos: uno en frente y dos en sus laterales. 

### Estados

Los estados de las hormigas cambian severamente el objetivo de la hormiga, las feromonas que siguen y las feromonas que dejan. Los tres estados posibles son:
- Idle: En este estado, la hormiga merodeara aleatoriamente en busca de comida. Además estará buscando feromonas de hormigas con comida, y en cuanto encuentran el rastro, comenzarán a seguir el rastro con más feromonas que puedan oler. Si el rastro desaparece sin haber encontrado comida, volverán a deambular. Si en cualquier momento son capaces de ver comida, se lanzarán a por ella directamente. Deja de estar en este estado al cambiar al estado de alerta, o al llegar al recoger comida.
- WithFood: En este estado, el objetivo de la hormiga será encontrar el hormiguero, yendo directamente hacia el en cuanto sea capaz de verlo. Si no lo ve, deambulará por el mapa hasta encontrar el rastro de una hormiga en el estado Idle, que seguirá en función de que sensor detecta mas feromonas. Deja de estar en este estado o al cambiar al estado de alerta o al llegar al hormiguero.
- Danger: En este estado, la hormiga actuará igual que en el estado WithFood, pero solo durante un tiempo limitado. Esto es así con el objetivo de que avisa al mayor numero de hormigas posible del peligro del camino del que viene (es por eso que va hacia el hormiguero). La hormiga será capaz de entrar en este estado cuando se quede en una misma localización mucho tiempo, o si encuentran un punto de peligro. Cambia al estado anterior al que estaba cuando pasa el tiempo de alerta o cuando alcanza el hormiguero.

### Deambular

El algoritmo de deambular se basa en guardar la dirección generada anteriormente y sumarle una dirección aleatoria con una magnitud parametrizable por el editor (la fuerza de deambular).

### Seguir feromonas

La hormiga es capaz de seguir y encontrar las feromonas gracias a tres sensores en su parte delantera: uno exactamente delante de ella, y otras dos en sus laterales. Estas detectan en un radio las feromonas de peligro (siempre independientemente del estado) y las objetivo, que si varian en función del estado. Una vez cada sensor ha calculado el valor de todos los sensores, se compará cual es el que tiene mayor valor entre ellos, y el que tenga mayor, será hacia donde la hormiga irá. Las feromonas de peligro tendrán un valor negativo en el estudio. Si los 3 escáneres dan valores negativos, la hormiga intentará dar la vuelta.

### Evitar obstáculos

La detección de obstáculos como ya se ha explicado se utilizará mediante tres raycasts: uno delante y dos a un ángulo del central. Además estos raycast tendrán 2 distancias: una de aviso, que aplicará una ligera desviación en su camino, y una mucho mas potente que evitará la colisión pero será mucho mas cercana. Además en función si ha detectado que colisionará o no con algo que tiene justo delante, calculará o no colisiones con los laterales para mejorar la eficiencia.

### Evitar atascamientos y vueltas en círculos cerrados

Durante el desarrollo del proyecto y durante la realización de las pruebas, me di cuenta de un comportamiento preocupante. Algunas hormigas quedaban atascadas indefinidamente en una única posición. Otras, cuando quedaban pocas o ninguna comida, empezaban a retroalimentarse unas a otras y eso generaba que empezarán a dar vueltas en círculos unas alrededor de las otras. Viendo esto, decidí implementar un algoritmo que evitaría que una hormiga se quedase cerca de una misma posición durante mucho tiempo. Esto lo conseguí guardando sus últimas dos posiciones cada una cantidad de tiempo determinada. Si al actualizar estas posiciones, se encontraba cerca de la posición mas antigua de las dos, quiere decir que ha estado girando en círculos o se ha quedado atascada en una posición por otros motivos. Por ello, decidí cambiar su comportamiento a un estado de alerta para así poder avisar a otras hormigas del peligro. Además por sus propias feromonas, está acaba escapando de su atascamiento, o rompe el circulo cerrado que se ha generado entre las hormigas, solucionando el problema. Cabe destacar, que en algunos casos, las hormigas forman círculos de retroalimentación mas grande que este rango. Pero estos círculos con el tiempo acaban disolviéndose ya que cada vez son más largos, hasta que las feromonas empiezan a afectar menos y menos y menos.

## Hormiguero

El hormiguero tiene una IA bastante sencilla pero muy parametrizada que intentará crear el mayor número de hormigas que pueda. Esto lo consigue, gastando el coste para producir una hormiga siempre que tenga suficiente para producir una nueva hormiga.

# Pruebas realizadas

Las pruebas realizadas para el estudio del funcionamiento de este proyecto fueron los siguientes:
- Con pocas hormigas, comida y un mapa pequeño, las hormigas actuan como esperado, aunque con las ultimas hormigas con comida suelen tardar bastante más en encontrar el camino de vuelta.
- Con un mapa más grande y varias fuentes de comida, las hormigas consiguen encontrar el camino de vuelta sin problema, hasta las últimas que en algunos casos tardan un poco más.
- Con un mapa con obstáculos, las hormigas son capaces de encontrar un camino esquivando los obstáculos, con la única dificultad de que si el camino es muy largo, se necesitan muchas hormigas para poder conseguir un comportamiento esperado, o feromonas que duren más tiempo. Cualquier de las dos soluciones suele requerir de un PC más potente.
- Si las hormigas no son capaces de encontrar comida, acabarán rellenando el mapa con patrones caóticos bastante estéticos a mi parecer.
- En el caso de colocar una comida en una posición imposible, las hormigas intentarán alcanzarla entrando en un bucle de intento, estado de alerta y de vuelta intentar alcanzar la comida. Si pasa el suficiente tiempo, todas las hormigas intentarán alcanzar esta comida bloqueada.

# Bibliografía y recursos utilizados

- Bolt, Visual Scripting
https://unity.com/es/products/unity-visual-scripting
- Opsive, Behavior Designer
https://opsive.com/assets/behavior-designer/
- Unity, Navegación y Búsqueda de caminos
https://docs.unity3d.com/es/2019.3/Manual/Navigation.html
- Unity 2018 Artificial Intelligence Cookbook, Second Edition (Repositorio)
https://github.com/PacktPublishing/Unity-2018-Artificial-Intelligence-Cookbook-Second-Edition
- Unity Artificial Intelligence Programming, Fourth Edition (Repositorio)
https://github.com/PacktPublishing/Unity-Artificial-Intelligence-Programming-Fourth-Edition
- Inspiraciones de implementaciones de simulaciones de hormigas:
https://www.youtube.com/watch?v=emRXBr5JvoY
https://www.youtube.com/watch?v=X-iSQQgOd1A
https://es.wikipedia.org/wiki/Formicidae
https://www.youtube.com/watch?v=3ilbVSQyE5A
https://www.youtube.com/watch?v=tPD9duy7PzM