
## Paleta de colores (CSS custom properties)


```css
:root {
  /* fondos */
  --color-parchment: #FAF8F5;        /* fondo principal */
  --color-parchment-dark: #F2ECE1;   /* tarjetas, paneles */
  --color-parchment-border: #E4DDD3; /* bordes suaves */

  /* texto */
  --color-ink: #2C2925;        /* texto principal */
  --color-ink-muted: #645F57;  /* texto secundario */
  --color-ink-light: #9B9489;  /* texto tenue, pistas */

  /* principal: botones, enlaces activos */
  --color-terracotta: #A64E3D;
  --color-terracotta-hover: #8D3E30;

  /* secundario: estados de éxito, toggles activos */
  --color-academy: #3F5245;
  --color-academy-hover: #314035;

  /* detalles, iconos, etiquetas */
  --color-ochre: #B0824B;
  --color-ochre-hover: #966E3E;
}
```

## Tipografías (Google Fonts)

Importar en el `_Layout.cshtml`:

- **Plus Jakarta Sans** — texto general (sans).
- **Playfair Display** — títulos grandes y números de juego (display, serif).
- **JetBrains Mono** — etiquetas, código, datos monoespaciados (mono).

```html
<link href="https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@300;400;500;600;700;800&family=Playfair+Display:ital,wght@0,400..905;1,400..905&family=JetBrains+Mono:wght@400;500;600;700&display=swap" rel="stylesheet">
```
