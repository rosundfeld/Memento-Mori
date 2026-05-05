# Memento Mori - GDD v0.0.2 (Implementavel)

## 1) Visao de Produto
Memento Mori e um jogo de estrategia com foco em decisao tatico-estrategica, nao em micro/APM.
O jogador e um Capelao: gerencia base e fe durante o dia, prepara formacao em grid e influencia o combate noturno com milagres.

### 1.1 High Concept operacional
"Gerencie a ultima resistencia, converta fe em vantagem de batalha e sobreviva a ciclos de dia/noite ate conquistar o nucleo demoniaco."

### 1.2 Pilares de Design
1. Decisao > Execucao: menos click por segundo, mais leitura de situacao.
2. Fe como sistema central: recurso emocional, economico e de combate.
3. Assistir a batalha com agencia: combate auto + intervencoes de alto impacto.
4. Escopo enxuto no inicio: primeiro provar loop, depois expandir conteudo.

## 2) Escopo do MVP (Obrigatorio)
### 2.1 Inclui
1. Campanha humana com 1 mapa macro e 5 nos de combate.
2. Ciclo Dia -> Preparacao -> Noite -> Pos-batalha.
3. 3 recursos: Ouro, Suprimento, Fe.
4. 6 unidades jogaveis.
5. 1 heroi jogavel.
6. 3 milagres ativos.
7. 5 tipos de inimigo.
8. 6 estruturas de base.
9. Vitoria/Derrota da run.

### 2.2 Nao inclui no MVP
1. Faccoes extras.
2. Multiplayer.
3. Leaderboard.
4. Modo hardcore.
5. Criador de bandeira.
6. DLC e metaprogressao extensa.

## 3) Core Loop Fechado
## 3.1 Estados de partida
1. Dia (Gestao): construir, reparar, treinar, alocar recursos.
2. Preparacao (Grid): posicionar unidades e definir stance inicial.
3. Noite (Combate): simulacao automatica + habilidades de fe + skills de unidade/heroi.
4. Pos-batalha: calcular perdas, ganhos, moral/fe, eventos, avancar no mapa.

### 3.2 Duracao alvo por ciclo (MVP)
1. Dia: 120s a 180s.
2. Preparacao: 30s a 45s.
3. Noite: 60s a 120s.
4. Pos-batalha: 15s a 25s.

### 3.3 Condicoes de vitoria e derrota
Vitoria de campanha (MVP): conquistar os 5 nos e destruir o nucleo demoniaco.
Derrota de campanha (MVP):
1. Fe chegar a 0 por 2 noites consecutivas, ou
2. Igreja Principal destruida, ou
3. Populacao util < 30% do inicial.

## 4) Sistemas com Regras Numericas
## 4.1 Fe (0 a 100)
### Ganhos
1. Vitoria em noite: +10
2. Vitoria sem perder heroi: +5 adicional
3. Ritual diario: +5 (cooldown de 1 ciclo)
4. Evento positivo: +3 a +8

### Perdas
1. Derrota em noite: -30
2. Morte de heroi: -12
3. Igreja danificada (>50%): -8
4. Evento negativo: -3 a -10

### Faixas e efeitos
1. 0-30 (Crise): -15% dano, -10% resistencia, -20% velocidade de recuperacao de habilidades.
2. 31-70 (Neutro): sem bonus/penalidade.
3. 71-100 (Inspirado): +10% dano, +10% resistencia, +15% eficacia de milagres.

## 4.2 Economia (MVP)
### Recursos
1. Ouro: construcoes, treino, upgrades.
2. Suprimento: limite de populacao militar/civil.
3. Fe: milagres e alguns efeitos de moral.

### Producoes base por ciclo (dia)
1. Fazenda: +12 suprimento
2. Deposito/Oficina: +20 ouro
3. Capela: +3 fe passiva

## 4.3 Estruturas do MVP
1. Igreja Principal: centro da base, derrota se destruida.
2. Quartel: infantaria base.
3. Estabulo: cavalaria.
4. Capela: ritual e recuperacao de fe.
5. Fazenda: suprimento.
6. Torre de Vigia: defesa automatica noturna.

## 4.4 Combate e Posicionamento
1. Grid 8x6 (jogador em 3 linhas de retaguarda+frente no MVP).
2. Cada unidade ocupa 1 celula.
3. Sem overlap.
4. Alvo definido por score (distancia + prioridade + aggro recente).
5. Re-target a cada 1.5s (valor inicial).

## 4.5 Intervencao do jogador em combate
1. Milagres (consomem fe).
2. Skills ativas de heroi/unidade (cooldown e, opcionalmente, custo de recurso secundario).
3. Ordem global da linha: Avancar, Manter, Recuar (cooldown 10s).

## 5) Tabela Inicial de Balanceamento (Seed)
## 5.1 Unidades do jogador
| Unidade | Vida | Dano | ASPD (ataques/s) | Alcance | Velocidade | Custo Ouro | Custo Suprimento | Funcao |
|---|---:|---:|---:|---:|---:|---:|---:|---|
| Miliciano | 120 | 14 | 0.9 | 1.2 | 2.8 | 30 | 1 | Frontline barato |
| Espadachim | 180 | 20 | 1.0 | 1.3 | 2.7 | 55 | 2 | Frontline padrao |
| Lanceiro | 150 | 18 | 0.95 | 1.5 | 2.6 | 50 | 2 | Anti-cavalaria |
| Besteiro | 95 | 26 | 0.75 | 5.5 | 2.5 | 70 | 2 | DPS alvo unico |
| Sacerdote | 110 | 8 | 0.7 | 4.5 | 2.4 | 80 | 2 | Cura/suporte |
| Cavaleiro | 240 | 24 | 0.85 | 1.4 | 3.1 | 110 | 3 | Engage/tanque |

## 5.2 Heroi inicial (Capitao Paladino)
1. Vida: 420
2. Dano: 32
3. ASPD: 0.9
4. Alcance: 1.4
5. Skill 1 - Escudo de Fe (8s CD): recebe -40% dano de projetil por 2.5s.
6. Skill 2 - Brado Sagrado (16s CD): aliados em 2.5 celulas recebem +15% dano por 4s.

## 5.3 Milagres (jogador)
1. Solo Consagrado
- Custo: 20 Fe
- Efeito: cura em area (3 celulas) de 12% da vida maxima em 4s.
- CD global: 14s

2. Chuva de Agua Benta
- Custo: 30 Fe
- Efeito: dano magico em area (3 celulas), 70 de dano total em 2s.
- CD global: 18s

3. Vinho da Alianca
- Custo: 25 Fe
- Efeito: +12% dano e +10% resistencia para 4 unidades-alvo por 6s.
- CD global: 20s

## 5.4 Ondas inimigas (MVP)
No 1: pressao leve (tutorial oculto)
No 2: adiciona ranged inimigo
No 3: pico de volume
No 4: elite com aura
No 5: chefe + invocacoes

## 6) UI/UX de Feedback (minimo jogavel)
1. Barra de Fe sempre visivel com cor por faixa (Crise/Neutro/Inspirado).
2. Telemetria clara no pos-batalha: Fe +/-, perdas, dano total, uptime de milagres.
3. Preview de area antes de conjurar milagre.
4. Tooltip com numeros (sem "efeito vago").

## 7) Arquitetura Tecnica (Unity)
## 7.1 Principios
1. Dados estaticos em ScriptableObject.
2. Estado runtime em classes separadas (nao mutar asset).
3. Sistemas desacoplados por eventos.
4. Atualizacoes periodicas para IA/targeting (nao buscar tudo por frame).

## 7.2 Modulos
1. GameFlowSystem: controla estados do ciclo.
2. FaithSystem: calcula ganhos/perdas e aplica faixas.
3. EconomySystem: ouro/suprimento/producao.
4. GridFormationSystem: posicao e validacao de celulas.
5. CombatSystem: tick de combate, dano, morte, fim de onda.
6. TargetingSystem: score de alvo e retarget.
7. AbilitySystem: cooldowns e execucao de skills/milagres.
8. WaveSystem: composicao e spawn de ondas.

## 7.3 Modelo de dados sugerido
1. UnitData (SO): nome, prefab, stats base, tags.
2. UnitRuntimeState (classe): vidaAtual, cooldowns, alvoAtual, moralLocal.
3. AbilityData (SO): custo, cd, raio, formulas.
4. BuildingData (SO): custo, prerequisitos, efeito.
5. WaveData (SO): lista de spawns por tempo.

## 7.4 Eventos centrais
1. OnBattleStart
2. OnBattleEnd
3. OnUnitDied
4. OnFaithChanged
5. OnStructureDamaged
6. OnMiracleCast

## 8) Backlog Tecnico por Fases
## Fase A - Vertical Slice (2 a 4 semanas)
1. Implementar maquina de estados Dia/Preparacao/Noite/Pos.
2. Grid 8x6 com posicionamento e validacao.
3. 3 unidades (Miliciano, Besteiro, Sacerdote).
4. Combate auto com ataque basico e range.
5. Fe com 3 faixas + 1 milagre (Solo Consagrado).
6. 1 onda inimiga + tela de resultado.

Criterio de aceite:
- Jogador completa 1 ciclo inteiro sem erro bloqueante.
- Resultado altera Fe e impacta proxima noite.

## Fase B - MVP jogavel (4 a 8 semanas)
1. Expandir para 6 unidades + heroi.
2. Adicionar 3 milagres completos.
3. Implementar 6 estruturas e economia base.
4. Adicionar 5 nos de campanha com dificuldade crescente.
5. Balanceamento inicial com telemetria.

Criterio de aceite:
- Run completa de 45-75 minutos, inicio ao fim.
- Pelo menos 3 estrategias validas de composicao.

## Fase C - Polimento Pre-Alpha
1. Melhorar FX/SFX de skills e milagres.
2. Melhorar readability da UI de combate.
3. Ajustar pacing de ciclos e curva de dificuldade.
4. Corrigir bugs de targeting/pathing/re-target.

Criterio de aceite:
- Testadores entendem causa de vitoria/derrota em >80% das partidas.

## 9) Riscos e Mitigacoes
1. Escopo inflado
- Mitigacao: congelar MVP; qualquer feature nova entra em pos-MVP.

2. IA cara/performance ruim
- Mitigacao: tick de decisao por intervalo (0.2-0.5s), cache de listas, evitar FindObjects em loop.

3. Fe ficar "so cosmetica"
- Mitigacao: garantir impacto numerico claro e perceptivel nas 3 faixas.

4. Combate virar caos visual
- Mitigacao: limitar numero de unidades simultaneas no MVP e padronizar VFX por prioridade.

## 10) Telemetria minima para balanceamento
1. Tempo medio por ciclo.
2. Taxa de vitoria por no.
3. Fe media por ciclo e distribuicao por faixa.
4. Milagre mais usado e taxa de acerto.
5. Unidades mais recrutadas e taxa de sobrevivencia.

## 11) Definicao de pronto (Definition of Done)
Uma feature so e considerada pronta quando:
1. Tem regra numerica documentada.
2. Possui feedback visual/sonoro minimo.
3. Foi testada em pelo menos 10 ciclos sem bug critico.
4. Possui log/telemetria basica para ajuste posterior.

## 12) Proxima revisao sugerida
Versao 0.0.3 deve incluir:
1. Tabela de progressao por no (inimigos, recompensas, dificuldade).
2. Tutorial de 3 minutos (onboarding).
3. Primeira passada de UX para leitura de combate.
