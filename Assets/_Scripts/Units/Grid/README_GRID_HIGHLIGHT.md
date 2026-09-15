# Grid Visual Highlight - Guia de Setup

## 🎨 Sistema de Highlight Visual do Grid (Padrão Xadrez)

Sistema que cria um padrão visual xadrez no grid para indicar área construível. **Desaparece automaticamente ao iniciar o combate**.

## 📦 Arquivos Criados/Modificados

1. **`GridVisualHighlight.cs`** - Componente que cria highlights visuais
2. **`GridFormationSystem.cs`** - Adicionados métodos públicos:
   - `GetGridInfo()` - Retorna dimensões e configurações do grid
   - `GetCellWorldPosition(x, y)` - Posição no mundo de uma célula

## 🚀 Setup Rápido (1 minuto)

### Opção 1: GameObject Separado (Recomendado)

```
1. Hierarchy → Create Empty → "GridHighlightSystem"
2. Add Component → GridVisualHighlight
3. No Inspector:
   - Color 1: Branco translúcido (R: 0.9, G: 0.9, B: 0.9, A: 0.3)
   - Color 2: Cinza translúcido (R: 0.8, G: 0.8, B: 0.8, A: 0.2)
   - Show During Combat: ☐ (desmarcado)
   - Cell Padding: 0.05
   - Grid Formation System: (arraste o GameObject com GridFormationSystem)
```

### Opção 2: Adicionar ao GridFormationSystem

```
1. Selecione o GameObject "GridFormationSystem" na Hierarchy
2. Add Component → GridVisualHighlight
3. Configure as cores no Inspector (mesmos valores acima)
```

## 🎨 Customização de Cores

### Presets Recomendados:

**Padrão Sutil (Atual):**
- Color 1: RGBA(0.9, 0.9, 0.9, 0.3) - Branco translúcido
- Color 2: RGBA(0.8, 0.8, 0.8, 0.2) - Cinza translúcido

**Verde/Azul (Área Construível):**
- Color 1: RGBA(0.5, 1.0, 0.5, 0.4) - Verde claro
- Color 2: RGBA(0.3, 0.8, 0.3, 0.3) - Verde escuro

**Azul/Roxo (Tema Místico):**
- Color 1: RGBA(0.6, 0.8, 1.0, 0.35) - Azul claro
- Color 2: RGBA(0.5, 0.6, 0.9, 0.25) - Azul escuro

**Amarelo/Laranja (Tema Fogo/Divino):**
- Color 1: RGBA(1.0, 0.95, 0.7, 0.3) - Amarelo claro
- Color 2: RGBA(0.9, 0.8, 0.5, 0.2) - Amarelo escuro

### Ajustar Transparência:
- **Mais Visível**: Aumente o valor Alpha (A) para 0.5 - 0.7
- **Mais Sutil**: Diminua o valor Alpha (A) para 0.1 - 0.2
- **Invisível**: Alpha = 0

### Ajustar Espaçamento:
- **Cell Padding**: 
  - `0.0` = células coladas (sem espaço)
  - `0.05` = espaço pequeno (padrão)
  - `0.1` = espaço maior (grid mais definido)

## 🎮 Comportamento

### Durante Planejamento (Antes do Combate):
- ✅ Grid highlight **visível**
- ✅ Padrão xadrez alternando Color 1 e Color 2
- ✅ Indica área construível

### Durante Combate:
- ❌ Grid highlight **escondido** automaticamente
- ❌ Células não são destruídas, apenas desativadas (SetActive(false))
- ✅ Performance mantida (sem recriação)

### Após Combate (Se voltar ao planejamento):
- ✅ Grid highlight **reaparece** automaticamente

## 🔧 Métodos Públicos

### Mudar Cores em Runtime:
```csharp
GridVisualHighlight highlight = FindObjectOfType<GridVisualHighlight>();
Color newColor1 = new Color(0.5f, 1f, 0.5f, 0.4f); // Verde claro
Color newColor2 = new Color(0.3f, 0.8f, 0.3f, 0.3f); // Verde escuro
highlight.UpdateColors(newColor1, newColor2);
```

### Forçar Mostrar/Esconder:
```csharp
GridVisualHighlight highlight = FindObjectOfType<GridVisualHighlight>();
highlight.SetVisible(false); // Esconde manualmente
highlight.SetVisible(true);  // Mostra manualmente
```

### Recriar Highlights (se grid mudar):
```csharp
GridVisualHighlight highlight = FindObjectOfType<GridVisualHighlight>();
highlight.RefreshHighlights();
```

## ⚙️ Como Funciona

### Inicialização:
1. Obtém referência ao `GridFormationSystem`
2. Aguarda 0.1s para garantir que o grid foi criado
3. Obtém dimensões do grid via `GetGridInfo()`
4. Cria GameObjects com `SpriteRenderer` para cada célula
5. Aplica padrão xadrez (alterna cores baseado em `(x + y) % 2`)
6. Posiciona cada célula no centro da célula do grid
7. Organiza todos em um GameObject pai "GridHighlights"

### Em Runtime:
- A cada frame, verifica `CombatManager.IsCombatActive`
- Se combate **inativo**: `SetActive(true)` nos highlights
- Se combate **ativo**: `SetActive(false)` nos highlights

### Performance:
- **Criação**: O(width × height) - uma vez no Start
- **Update**: O(1) - apenas checa estado do combate
- **Visibilidade**: O(n) onde n = número de células (apenas quando estado muda)
- **Memória**: Baixa - reutiliza GameObjects (não recria)

## 🐛 Troubleshooting

**Highlights não aparecem:**
- Verifique se `GridFormationSystem` está na cena e atribuído
- Confirme que combate está **inativo** (`IsCombatActive = false`)
- Veja Console para erros durante inicialização
- Verifique se Camera está posicionada para ver o grid

**Cores muito transparentes/invisíveis:**
- Aumente o valor Alpha das cores (Color 1 e Color 2)
- Valores recomendados: A entre 0.3 e 0.5

**Highlights aparecem durante combate:**
- Marque `Show During Combat = false` no Inspector
- Confirme que `CombatManager.Instance.IsCombatActive` retorna `true`

**Highlights desalinhados:**
- Verifique `Origin Position` do `GridFormationSystem`
- Confirme que `Cell Size` está correto
- Ajuste `Cell Padding` se necessário

**Performance baixa:**
- Reduza transparência (Alpha) se usar muitas camadas visuais
- Considere aumentar `Cell Padding` para reduzir sobreposição
- Desabilite em dispositivos low-end via script

## 💡 Dicas e Truques

### Animação de Fade In/Out:
Adicione ao `SetHighlightsVisible`:
```csharp
// No lugar de SetActive, use:
foreach (GameObject highlight in highlightObjects)
{
    SpriteRenderer sr = highlight.GetComponent<SpriteRenderer>();
    if (sr != null)
    {
        StartCoroutine(FadeSprite(sr, visible ? 1f : 0f));
    }
}

IEnumerator FadeSprite(SpriteRenderer sr, float targetAlpha)
{
    float startAlpha = sr.color.a;
    float elapsed = 0f;
    float duration = 0.3f;
    
    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
        Color c = sr.color;
        c.a = alpha;
        sr.color = c;
        yield return null;
    }
}
```

### Indicar Células Ocupadas:
Modifique `OnObjectPlaced`:
```csharp
private void OnObjectPlaced(object sender, System.EventArgs e)
{
    // Escurecer células ocupadas
    for (int x = 0; x < gridInfo.width; x++)
    {
        for (int y = 0; y < gridInfo.height; y++)
        {
            Unit unit = gridFormationSystem.GetUnitAtCell(new Vector2Int(x, y));
            int index = y * gridInfo.width + x;
            
            if (unit != null && index < highlightObjects.Count)
            {
                SpriteRenderer sr = highlightObjects[index].GetComponent<SpriteRenderer>();
                if (sr != null) sr.color = new Color(0.5f, 0.5f, 0.5f, 0.1f); // Cinza escuro
            }
        }
    }
}
```

### Diferentes Camadas Visuais:
Ajuste `sortingOrder` no `CreateCellHighlight`:
- `-10`: Atrás de tudo (padrão)
- `-5`: Atrás de unidades mas na frente do fundo
- `0`: Mesmo nível das unidades
- `5`: Na frente de unidades

### Borders/Contornos:
Adicione um segundo SpriteRenderer com escala menor para criar contorno:
```csharp
// No CreateCellHighlight, após criar o SpriteRenderer:
GameObject border = new GameObject("Border");
border.transform.SetParent(cellObj.transform);
border.transform.localPosition = Vector3.zero;
SpriteRenderer borderSR = border.AddComponent<SpriteRenderer>();
borderSR.sprite = CreateSquareSprite();
borderSR.sortingOrder = -9; // Na frente do highlight
borderSR.color = new Color(0.3f, 0.3f, 0.3f, 0.5f); // Cinza escuro
border.transform.localScale = Vector3.one * 1.05f; // Ligeiramente maior
```

## ✅ Checklist de Integração

- [ ] `GridVisualHighlight` adicionado à cena
- [ ] `GridFormationSystem` atribuído no Inspector
- [ ] Cores configuradas (Color 1 e Color 2)
- [ ] `Show During Combat` desmarcado
- [ ] Testado em play mode: highlights visíveis antes do combate
- [ ] Testado: highlights desaparecem ao iniciar combate
- [ ] (Opcional) Cores personalizadas aplicadas
- [ ] (Opcional) Cell Padding ajustado

---

**Pronto!** Grid visual xadrez funcional que desaparece durante combate. 🎨✨
