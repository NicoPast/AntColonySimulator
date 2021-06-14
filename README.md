# AntColonySimulator

# Descripción del proyecto

Este proyecto demuestra el funcionamiento de una colonia de hormigas muy simplificadamente. Comenzará con unas pocas hormigas que buscarán comida por un mapa de manera aleatoria partiendo desde su hormiguero. De ahí, siguiendo un modelo que asemeja la realidad, estas irán dejando un rastro de feromonas para poder regresar al hormiguero, o guiar a otras hormigas a encontrar la comida que han encontrado. A medida que estas hormigas vayan consiguiendo introducir comida en el hormiguero, más hormigas podrán ir naciendo. Además si encuentran peligros, dejarán unas feromonas que hará que el resto de hormigas eviten ese lugar. Al encontrar esta comida, intentaran volver a su hormiguero a depositar lo encontrado y permitir que más hormigas nazcan.

# Descripción de la escena

La escena representa un modelo de terrario horizontal, teniendo unos bordes marcados y obstaculos que se podrán ir expandiendo.

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

Se ven como cuadrados como de una especie de lava.

## Muros

## Interfaz

## Controles del jugador

# Descripción de la IA

## Hormigas

### Estados

### Deambular

### Seguir feromonas

### Evitar obstaculos

## Hormiguero

# Pruebas realizadas

# Bibliografía y recursos utilizados