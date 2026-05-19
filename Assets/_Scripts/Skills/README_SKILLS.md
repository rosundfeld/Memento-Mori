# Sistema de Skills - Memento Mori

Sistema completo de skills estilo Team Fight Tactics com recurso de Fé, cooldowns globais, targeting baseado em grid e efeitos de área/buff.

## 📦 Arquivos Criados

### Core System
- **`SkillDataSO.cs`** - ScriptableObject que define uma skill (nome, custo, cooldown, efeito)
- **`SkillEffect.cs`** - Classe base abstrata + 3 efeitos implementados:
  - `HealAreaEffect` - Cura em área ao longo do tempo
  - `DamageAreaEffect` - Dano mágico em área ao longo do tempo
  - `BuffEffect` - Buff de dano e resistência em alvos múltiplos
- **`SkillsManager.cs`** - Gerenciador central (Fé, cooldowns, execução de skills)

### UI Components
- **`SkillButton.cs`** - Componente para botão de skill individual (ícone, custo, cooldown visual)
- **`FaithDisplay.cs`** - Display do recurso de Fé (texto + barra)

## 🚀 Setup Rápido (5 minutos)

### 1. Criar Efeitos de Skill (ScriptableObjects)

No Unity, crie 3 assets de efeitos:

**Solo Consagrado - Heal Effect:**
```
Right-click → Create → Skills/Effects/Heal Area
Nome: "HealEffect_SoloConsagrado"
- Heal Percentage: 0.12 (12%)
- Duration: 4
- Area Radius: 1 (3x3 células)
```

**Chuva de Água Benta - Damage Effect:**
```
Right-click → Create → Skills/Effects/Damage Area
Nome: "DamageEffect_ChuvaAguaBenta"
- Total Damage: 70
- Duration: 2
- Area Radius: 1
```

**Vinho da Aliança - Buff Effect:**
```
Right-click → Create → Skills/Effects/Buff
Nome: "BuffEffect_VinhoAlianca"
- Damage Bonus: 0.12 (12%)
- Resistance Bonus: 0.10 (10%)
- Target Count: 4
- Duration: 6
```

### 2. Criar Skills (ScriptableObjects)

Crie 3 assets de skills:

**Skill 1: Solo Consagrado**
```
Right-click → Create → Skills/Skill Data
Nome: "Skill_SoloConsagrado"
- Skill Name: "Solo Consagrado"
- Description: "Abençoa o solo, curando aliados na área"
- Faith Cost: 20
- Cooldown: 14
- Target Range: 5
- Requires Combat: true
- Effect: (arraste HealEffect_SoloConsagrado)
```

**Skill 2: Chuva de Água Benta**
```
Nome: "Skill_ChuvaAguaBenta"
- Skill Name: "Chuva de Água Benta"
- Description: "Invoca chuva abençoada que queima inimigos"
- Faith Cost: 30
- Cooldown: 18
- Target Range: 5
- Effect: (arraste DamageEffect_ChuvaAguaBenta)
```

**Skill 3: Vinho da Aliança**
```
Nome: "Skill_VinhoAlianca"
- Skill Name: "Vinho da Aliança"
- Description: "Fortalece aliados com bênção divina"
- Faith Cost: 25
- Cooldown: 20
- Target Range: 5
- Effect: (arraste BuffEffect_VinhoAlianca)
```

### 3. Setup de Cena

**A) Criar SkillsManager GameObject:**
```
1. Hierarchy → Create Empty → "SkillsManager"
2. Add Component → SkillsManager
3. No Inspector:
   - Max Faith: 100
   - Current Faith: 100
   - Faith Regen Per Second: 2
   - Available Skills: (arraste as 3 skills criadas)
   - Grid System: (arraste o GameObject com GridFormationSystem)
```

**B) Criar UI de Skills:**
```
1. Canvas → Create Empty → "SkillsPanel"
2. Dentro dele:
   - Create → UI → Button (renomeie para "Skill1_Button")
   - Duplique 2x para ter 3 botões (Skill2_Button, Skill3_Button)
```

**C) Configurar cada botão:**
```
Para cada botão de skill:
1. Add Component → SkillButton
2. No Inspector:
   - Skill: (arraste Skill_SoloConsagrado / ChuvaAguaBenta / VinhoAlianca)
   - Icon Image: (arraste a Image child do botão)
   - Cost Text: (crie um Text/TextMeshPro como child e arraste)
   - Cooldown Text: (crie outro Text para mostrar segundos)
   - Cooldown Overlay: (opcional - crie uma Image com fillAmount para overlay visual)
   - Button: (auto-preenchido)
```

**D) Criar Display de Fé:**
```
1. Canvas → Create → UI → Panel → "FaithPanel"
2. Dentro dele:
   - UI → Text (TextMeshPro) → "FaithText"
   - UI → Image → "FaithBar" (configure como fillable horizontal)
3. No FaithPanel, Add Component → FaithDisplay
4. Configure:
   - Faith Text: (arraste FaithText)
   - Faith Bar: (arraste FaithBar)
   - Faith Format: "{0} / {1} Fé"
```

## 🎮 Como Usar

### Em Jogo:

1. **Iniciar Combate**: Clique no botão "Start Combat" (do sistema anterior)
2. **Usar Skill**: Clique em um dos botões de skill
3. **Selecionar Alvo**: Clique em uma célula do grid para aplicar o efeito
4. **Cancelar**: Botão direito ou ESC cancela a seleção
5. **Cooldown**: Aguarde o cooldown terminar para usar novamente

### Mecânicas:

- **Fé**: Regenera 2 por segundo automaticamente (até 100)
- **Cooldown Global**: Cada skill tem seu próprio cooldown independente
- **Targeting**: Clique na célula desejada após ativar a skill
- **Área de Efeito**: Skills afetam células adjacentes (configurável por skill)
- **Validações**: Sistema impede uso se:
  - Fé insuficiente
  - Skill em cooldown
  - Combate não iniciado (se `requiresCombat = true`)

## 🔧 Customização

### Criar Nova Skill:

1. **Criar Efeito** (se necessário):
```csharp
[CreateAssetMenu(fileName = "MeuEfeito", menuName = "Skills/Effects/Meu Efeito")]
public class MeuEfeito : SkillEffect
{
    public override void Execute(Vector2Int targetCell, GridFormationSystem grid, SkillsManager skillManager)
    {
        // Sua lógica aqui
    }

    public override string GetDescription()
    {
        return "Descrição do efeito";
    }
}
```

2. **Criar Asset do Efeito** no Unity

3. **Criar Asset de Skill** (Skills/Skill Data) e configurar propriedades

4. **Adicionar à lista** `availableSkills` do SkillsManager

### Ajustar Balance:

No Inspector de cada Skill, ajuste:
- `Faith Cost` - Custo em Fé
- `Cooldown` - Tempo de recarga em segundos
- `Target Range` - Alcance de seleção (0 = auto-target)

No Inspector de cada Efeito, ajuste:
- Heal/Damage: porcentagens, duração, raio
- Buff: bônus, número de alvos, duração

### Adicionar Ícones:

1. Importe sprites de ícones para o projeto
2. No asset da Skill, arraste o sprite para o campo `Icon`
3. O ícone aparecerá automaticamente no botão UI

## 🐛 Troubleshooting

**Botão não aparece/fica desabilitado:**
- Verifique se `SkillsManager` está na cena e tem as skills atribuídas
- Confirme que `CombatManager.Instance.IsCombatActive` é `true` (se `requiresCombat = true`)
- Verifique Fé atual no Inspector do SkillsManager

**Efeito não funciona:**
- Confirme que `GridFormationSystem` está atribuído no SkillsManager
- Verifique se Units têm o componente `Unit` com stats configurados
- Veja logs no Console (habilite "Info" para ver mensagens de debug)

**Cooldown não aparece:**
- Confirme que `Cooldown Text` e `Cooldown Overlay` estão atribuídos no SkillButton
- Verifique se a Image do overlay tem `Image Type = Filled` e `Fill Method = Radial 360`

**Unit não recebe efeito:**
- `GetUnitAtCell` retorna null se não houver Unit na célula
- Confirme que `PlacedObject_Done` tem componente `Unit` anexado
- Veja logs no Console para debug de quantas unidades foram afetadas

## 📚 Arquitetura

```
SkillDataSO (asset)
    └─ SkillEffect (asset)
         └─ HealAreaEffect / DamageAreaEffect / BuffEffect

SkillsManager (singleton)
    ├─ Gerencia Fé (currentFaith, maxFaith, regen)
    ├─ Gerencia Cooldowns (Dictionary<string, float>)
    ├─ Executa Skills (valida → gasta Fé → aplica efeito → inicia CD)
    └─ Emite Eventos (OnFaithChanged, OnCooldownStarted, etc.)

SkillButton (UI)
    ├─ Escuta eventos do SkillsManager
    ├─ Atualiza visuals (ícone, custo, cooldown)
    └─ Chama SkillsManager.TryUseSkill() ao clicar

FaithDisplay (UI)
    └─ Escuta OnFaithChanged e atualiza barra/texto
```

## ✅ Checklist de Integração

- [ ] SkillsManager adicionado à cena com skills configuradas
- [ ] GridFormationSystem atribuído no SkillsManager
- [ ] 3 botões de skill criados com componente SkillButton
- [ ] FaithDisplay criado e configurado
- [ ] Assets de SkillDataSO criados (3 skills do GDD)
- [ ] Assets de SkillEffect criados (heal, damage, buff)
- [ ] Ícones atribuídos às skills (opcional mas recomendado)
- [ ] Testado em play mode: usar skill, ver efeito, aguardar cooldown

## 🎯 Próximos Passos (Opcional)

- [ ] Adicionar VFX/SFX ao usar skills
- [ ] Implementar indicador visual de área de efeito no grid
- [ ] Adicionar animação de cast (barra de progresso)
- [ ] Criar sistema de unlock progressivo de skills
- [ ] Adicionar tooltips com descrição completa ao hover
- [ ] Implementar keybinds (1, 2, 3 para usar skills)
- [ ] Salvar/carregar cooldowns ativos entre cenas

---

**Dúvidas?** Verifique logs no Console (Window → General → Console) com filtro "Info" habilitado para ver mensagens de debug do sistema.
