# Recomendaciones para hacer buenos tests

Este proyecto tiene varios tests que mezclan demasiada lógica o verifican detalles internos. Para mantenerlos útiles y fáciles de mantener, conviene seguir estas reglas:

1. Probar un comportamiento claro por test.
2. Usar nombres explícitos con formato `Metodo_Condicion_Resultado`.
3. Separar bien `Arrange`, `Act` y `Assert`.
4. Validar el contrato visible, no la implementación interna.
5. En controladores, comprobar el `IActionResult`, el contenido de la respuesta y los efectos secundarios relevantes como cookies o headers.
6. En handlers o servicios, verificar entradas, salida y las llamadas importantes a dependencias externas.
7. Evitar mocks innecesarios; solo mockear bordes del sistema como repositorios, correo, caché o almacenamiento.
8. No dejar tests desactualizados cuando cambie la firma de un handler o la forma de persistir datos.
9. Cubrir al menos un caso feliz y uno de error por endpoint crítico.
10. Usar datos de prueba realistas y mínimos para que la intención del test sea obvia.
11. Verificar que una dependencia no se invoque cuando la validación falla.
12. Si el test necesita demasiado setup, considerar crear helpers o builders compartidos.

## Ejemplo práctico

Para un endpoint de login, un buen test debería comprobar que:

- responde con `200 OK` cuando las credenciales son válidas,
- devuelve el payload esperado,
- deja la cookie de acceso con los flags correctos,
- no ejecuta el flujo cuando el modelo es inválido.

Eso da más valor que un test que solo confirme que se llamó a un método interno.