using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using RpgLibrary.Combat;
using RpgLibrary.Contracts;

namespace RpgWinForm
{
    // Turn-based battle window. BattleSystem runs on a background thread
    // and calls back to us through WinFormBattleUI.
    public class BattleForm : Form
    {
        // theme colours
        private static readonly Color BgDark    = Color.FromArgb(20, 22, 40);
        private static readonly Color PanelDark = Color.FromArgb(30, 34, 60);
        private static readonly Color HeroColor = Color.FromArgb(120, 210, 255);
        private static readonly Color EnemyColor= Color.FromArgb(255, 110, 120);
        private static readonly Color GoldColor = Color.FromArgb(255, 200, 90);
        private static readonly Color TextColor = Color.FromArgb(230, 232, 245);
        private static readonly Color MutedColor= Color.FromArgb(140, 150, 175);

        private readonly List<PartyMember> _party;
        private readonly List<ICombatant> _enemies;
        private WinFormBattleUI _ui = null!;
        private BattleSystem _battle = null!;
        private Task? _battleTask;

        private Label _turnBanner = null!;
        private FlowLayoutPanel _partyStack = null!;
        private FlowLayoutPanel _enemyStack = null!;
        private TextBox _logBox = null!;
        private FlowLayoutPanel _actionBar = null!;
        private Label _actionPrompt = null!;

        private readonly Dictionary<ICombatant, CombatantCard> _cards = new();

        public BattleForm(List<PartyMember> party, List<ICombatant> enemies)
        {
            _party = party;
            _enemies = enemies;

            Text = "Battle Arena";
            Size = new Size(1200, 720);
            MinimumSize = new Size(1000, 620);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = BgDark;
            ForeColor = TextColor;
            Font = new Font("Segoe UI", 10F);

            BuildUi();
            RegisterCombatants();

            Load += (_, _) => StartBattle();
            FormClosing += (_, _) => { /* battle thread ends when form dies */ };
        }

        private void BuildUi()
        {
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1,
                BackColor = BgDark,
                Padding = new Padding(12),
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 130));
            Controls.Add(root);

            // Turn banner
            _turnBanner = new Label
            {
                Text = "Preparing battle...",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = GoldColor,
                BackColor = PanelDark,
            };
            root.Controls.Add(_turnBanner, 0, 0);

            // Battlefield row: party | log | enemies
            var battlefield = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 1,
                ColumnCount = 3,
                BackColor = BgDark,
                Margin = new Padding(0, 8, 0, 8),
            };
            battlefield.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 27));
            battlefield.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 46));
            battlefield.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 27));
            root.Controls.Add(battlefield, 0, 1);

            battlefield.Controls.Add(BuildPartyPanel(), 0, 0);
            battlefield.Controls.Add(BuildLogPanel(),   1, 0);
            battlefield.Controls.Add(BuildEnemyPanel(), 2, 0);

            // Action bar (bottom)
            root.Controls.Add(BuildActionBar(), 0, 2);
        }

        private GroupBox BuildPartyPanel()
        {
            var group = MakeGroup("PARTY", HeroColor);
            _partyStack = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = true,
                WrapContents = false,
                BackColor = PanelDark,
            };
            group.Controls.Add(_partyStack);
            return group;
        }

        private GroupBox BuildEnemyPanel()
        {
            var group = MakeGroup("ENEMIES", EnemyColor);
            _enemyStack = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = true,
                WrapContents = false,
                BackColor = PanelDark,
            };
            group.Controls.Add(_enemyStack);
            return group;
        }

        private GroupBox BuildLogPanel()
        {
            var group = MakeGroup("BATTLE LOG", GoldColor);
            _logBox = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                Dock = DockStyle.Fill,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 9.5F),
                BackColor = Color.FromArgb(15, 17, 30),
                ForeColor = TextColor,
                BorderStyle = BorderStyle.None,
                WordWrap = true,
            };
            group.Controls.Add(_logBox);
            return group;
        }

        private Panel BuildActionBar()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = PanelDark,
                Padding = new Padding(12),
            };

            _actionPrompt = new Label
            {
                Text = "",
                Dock = DockStyle.Top,
                Height = 28,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = MutedColor,
                TextAlign = ContentAlignment.MiddleLeft,
            };

            _actionBar = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = PanelDark,
                AutoScroll = true,
            };

            panel.Controls.Add(_actionBar);
            panel.Controls.Add(_actionPrompt);
            return panel;
        }

        private GroupBox MakeGroup(string title, Color accent)
        {
            return new GroupBox
            {
                Text = title,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = accent,
                Padding = new Padding(6),
            };
        }

        private void RegisterCombatants()
        {
            foreach (PartyMember p in _party)
            {
                var card = new CombatantCard(p.Combatant, p, HeroColor);
                _cards[p.Combatant] = card;
                _partyStack.Controls.Add(card);
            }
            foreach (ICombatant e in _enemies)
            {
                var card = new CombatantCard(e, null, EnemyColor);
                _cards[e] = card;
                _enemyStack.Controls.Add(card);
            }
        }

        private void StartBattle()
        {
            // Route all Console.WriteLine from the library into the log too.
            Console.SetOut(new TextBoxWriter(_logBox));

            _ui = new WinFormBattleUI(this);
            _battle = new BattleSystem(
                _party,
                _enemies,
                ui: _ui,
                enemyStrategy: new RandomTargetStrategy(),
                settings: new BattleSettings { UltimateChargePerTurn = 1, MaxRounds = 30 });

            // Run the battle loop on a background thread so it can block on
            // IBattleUI choices without freezing the UI.
            _battleTask = Task.Run(() => _battle.Run());
        }

        // Everything below is called by WinFormBattleUI from the battle thread.
        // We must Invoke back onto the UI thread.

        public void UiRefreshState(BattleState state)
        {
            if (InvokeRequired) { BeginInvoke(new Action(() => UiRefreshState(state))); return; }

            foreach (var card in _cards.Values) card.UpdateCard();

            if (state.TurnOrder.Count > 0)
                _turnBanner.Text = $"Round {state.Round}   -   Turn order: " + JoinTurnOrder(state);
        }

        public void UiShowTurn(ICombatant actor, bool isHero)
        {
            if (InvokeRequired) { BeginInvoke(new Action(() => UiShowTurn(actor, isHero))); return; }

            _turnBanner.Text = $"{actor.Name}'s turn   (Speed {actor.Speed})";
            _turnBanner.ForeColor = isHero ? HeroColor : EnemyColor;

            // Highlight the active card
            foreach (var kvp in _cards)
                kvp.Value.SetActive(kvp.Key == actor);
        }

        public void UiAppendLog(string message)
        {
            if (InvokeRequired) { BeginInvoke(new Action(() => UiAppendLog(message))); return; }
            _logBox.AppendText(message + Environment.NewLine);
        }

        // onChosen fires when the player clicks a button, unblocking WinFormBattleUI
        public void UiPromptAction(PartyMember member,
                                   IReadOnlyList<BattleActionOption> options,
                                   Action<BattleActionOption?> onChosen)
        {
            if (InvokeRequired) { BeginInvoke(new Action(() => UiPromptAction(member, options, onChosen))); return; }

            _actionPrompt.Text = $"{member.Combatant.Name}, choose your action:";
            _actionPrompt.ForeColor = HeroColor;
            _actionBar.Controls.Clear();

            foreach (BattleActionOption opt in options)
            {
                Button btn = MakeActionButton(opt);
                BattleActionOption captured = opt;
                btn.Click += (_, _) =>
                {
                    ClearActionBar();
                    onChosen(captured);
                };
                _actionBar.Controls.Add(btn);
            }
        }

        public void UiPromptTarget(List<ICombatant> candidates,
                                   string prompt,
                                   Action<ICombatant?> onChosen)
        {
            if (InvokeRequired) { BeginInvoke(new Action(() => UiPromptTarget(candidates, prompt, onChosen))); return; }

            _actionPrompt.Text = $"{prompt}:";
            _actionPrompt.ForeColor = GoldColor;
            _actionBar.Controls.Clear();

            foreach (ICombatant c in candidates)
            {
                Button btn = new Button
                {
                    Text = $"{c.Name}  ({c.Health}/{c.MaxHealth})",
                    AutoSize = false,
                    Size = new Size(180, 60),
                    BackColor = Color.FromArgb(60, 40, 40),
                    ForeColor = TextColor,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    FlatStyle = FlatStyle.Flat,
                    Margin = new Padding(4),
                };
                btn.FlatAppearance.BorderColor = EnemyColor;
                ICombatant captured = c;
                btn.Click += (_, _) =>
                {
                    ClearActionBar();
                    onChosen(captured);
                };
                _actionBar.Controls.Add(btn);
            }
        }

        public void UiPromptSkill(List<Skill> skills, Action<Skill?> onChosen)
        {
            if (InvokeRequired) { BeginInvoke(new Action(() => UiPromptSkill(skills, onChosen))); return; }

            _actionPrompt.Text = "Choose a skill:";
            _actionPrompt.ForeColor = GoldColor;
            _actionBar.Controls.Clear();

            foreach (Skill s in skills)
            {
                Button btn = new Button
                {
                    Text = $"{s.Name}\n(power {s.Power}, {s.Target})",
                    AutoSize = false,
                    Size = new Size(180, 60),
                    BackColor = Color.FromArgb(40, 60, 60),
                    ForeColor = TextColor,
                    Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                    FlatStyle = FlatStyle.Flat,
                    Margin = new Padding(4),
                };
                btn.FlatAppearance.BorderColor = HeroColor;
                Skill captured = s;
                btn.Click += (_, _) =>
                {
                    ClearActionBar();
                    onChosen(captured);
                };
                _actionBar.Controls.Add(btn);
            }
        }

        public void UiShowEnd(BattleOutcome outcome, int rounds)
        {
            if (InvokeRequired) { BeginInvoke(new Action(() => UiShowEnd(outcome, rounds))); return; }

            _turnBanner.Text = outcome switch
            {
                BattleOutcome.Victory => $"VICTORY - party wins in {rounds} rounds",
                BattleOutcome.Defeat  => $"DEFEAT - party wiped out after {rounds} rounds",
                BattleOutcome.Timeout => $"TIMEOUT - round limit reached ({rounds} rounds)",
                _                     => $"UNDECIDED",
            };
            _turnBanner.ForeColor = outcome == BattleOutcome.Victory ? HeroColor : EnemyColor;
            _actionPrompt.Text = "";
            _actionBar.Controls.Clear();

            foreach (var card in _cards.Values) card.SetActive(false);
        }

        private void ClearActionBar()
        {
            _actionBar.Controls.Clear();
            _actionPrompt.Text = "";
            _actionPrompt.ForeColor = MutedColor;
        }

        private Button MakeActionButton(BattleActionOption opt)
        {
            Color face = opt.Kind switch
            {
                BattleActionKind.Attack   => Color.FromArgb(70, 90, 130),
                BattleActionKind.Skill    => Color.FromArgb(60, 110, 130),
                BattleActionKind.Ultimate => Color.FromArgb(140, 90, 40),
                BattleActionKind.Defend   => Color.FromArgb(60, 80, 90),
                _                          => Color.FromArgb(80, 80, 80),
            };

            string caption = opt.Detail != null
                ? $"{opt.Label}\n{opt.Detail}"
                : opt.Label;

            var btn = new Button
            {
                Text = caption,
                AutoSize = false,
                Size = new Size(180, 60),
                BackColor = opt.Enabled ? face : Color.FromArgb(50, 50, 60),
                ForeColor = opt.Enabled ? TextColor : MutedColor,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(4),
                Enabled = opt.Enabled,
            };
            btn.FlatAppearance.BorderColor = opt.Kind == BattleActionKind.Ultimate ? GoldColor : HeroColor;
            btn.FlatAppearance.BorderSize = 2;
            return btn;
        }

        private string JoinTurnOrder(BattleState state)
        {
            string s = "";
            for (int i = 0; i < state.TurnOrder.Count; i++)
            {
                if (i > 0) s += "  →  ";
                s += $"{state.TurnOrder[i].Name}({state.TurnOrder[i].Speed})";
            }
            return s;
        }
    }

    // One card per fighter: name, HP bar, ultimate bar.
    internal class CombatantCard : Panel
    {
        private readonly ICombatant _combatant;
        private readonly PartyMember? _member;
        private readonly Color _accent;

        private Label _nameLabel = null!;
        private Label _hpLabel = null!;
        private Panel _hpBar = null!;
        private Label _ultLabel = null!;
        private Panel _ultBar = null!;

        public CombatantCard(ICombatant combatant, PartyMember? member, Color accent)
        {
            _combatant = combatant;
            _member = member;
            _accent = accent;

            Width = 260;
            Height = 100;
            Margin = new Padding(6);
            BackColor = Color.FromArgb(40, 45, 70);
            BorderStyle = BorderStyle.FixedSingle;

            BuildInner();
            UpdateCard();
        }

        private void BuildInner()
        {
            _nameLabel = new Label
            {
                Location = new Point(10, 6),
                Size = new Size(240, 22),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = _accent,
                BackColor = Color.Transparent,
            };

            _hpLabel = new Label
            {
                Location = new Point(10, 32),
                Size = new Size(240, 16),
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.FromArgb(220, 220, 230),
                BackColor = Color.Transparent,
            };

            _hpBar = new Panel
            {
                Location = new Point(10, 50),
                Size = new Size(240, 14),
                BackColor = Color.FromArgb(20, 20, 30),
            };
            _hpBar.Paint += (s, e) => DrawHpBar(e.Graphics);

            _ultLabel = new Label
            {
                Location = new Point(10, 66),
                Size = new Size(240, 14),
                Font = new Font("Segoe UI", 8F),
                ForeColor = Color.FromArgb(255, 200, 90),
                BackColor = Color.Transparent,
            };

            _ultBar = new Panel
            {
                Location = new Point(10, 82),
                Size = new Size(240, 10),
                BackColor = Color.FromArgb(20, 20, 30),
            };
            _ultBar.Paint += (s, e) => DrawUltBar(e.Graphics);

            Controls.Add(_nameLabel);
            Controls.Add(_hpLabel);
            Controls.Add(_hpBar);
            Controls.Add(_ultLabel);
            Controls.Add(_ultBar);
        }

        public void UpdateCard()
        {
            _nameLabel.Text = _combatant.IsAlive() ? _combatant.Name : $"{_combatant.Name}  (KO)";
            _hpLabel.Text = $"HP  {_combatant.Health} / {_combatant.MaxHealth}";
            _hpBar.Invalidate();

            UltimateSkill? ult = _member?.Ultimate ?? (_combatant is Boss b ? b.Ultimate : null);
            if (ult != null)
            {
                _ultLabel.Visible = true;
                _ultBar.Visible = true;
                _ultLabel.Text = ult.IsCharged
                    ? $"ULT  READY ({ult.Name})"
                    : $"ULT  {ult.CurrentCharge} / {ult.MaxCharge}  ({ult.Name})";
                _ultBar.Invalidate();
            }
            else
            {
                _ultLabel.Visible = false;
                _ultBar.Visible = false;
            }

            BackColor = _combatant.IsAlive()
                ? Color.FromArgb(40, 45, 70)
                : Color.FromArgb(30, 25, 30);
        }

        public void SetActive(bool active)
        {
            BackColor = active
                ? Color.FromArgb(60, 75, 110)
                : _combatant.IsAlive() ? Color.FromArgb(40, 45, 70) : Color.FromArgb(30, 25, 30);
        }

        private void DrawHpBar(Graphics g)
        {
            double frac = _combatant.MaxHealth <= 0 ? 0 : (double)_combatant.Health / _combatant.MaxHealth;
            if (frac < 0) frac = 0;
            if (frac > 1) frac = 1;

            int fillWidth = (int)(_hpBar.Width * frac);
            if (fillWidth <= 0) return;

            // Colour by HP fraction: green → yellow → red
            Color fillColor = frac > 0.5 ? Color.FromArgb(80, 210, 120)
                            : frac > 0.25 ? Color.FromArgb(240, 210, 90)
                            : Color.FromArgb(230, 90, 90);

            using (var brush = new LinearGradientBrush(
                new Rectangle(0, 0, fillWidth, _hpBar.Height),
                fillColor, ControlPaint.Dark(fillColor, 0.2f),
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, 0, 0, fillWidth, _hpBar.Height);
            }
        }

        private void DrawUltBar(Graphics g)
        {
            UltimateSkill? ult = _member?.Ultimate ?? (_combatant is Boss b ? b.Ultimate : null);
            if (ult == null) return;

            double frac = ult.MaxCharge <= 0 ? 0 : (double)ult.CurrentCharge / ult.MaxCharge;
            if (frac < 0) frac = 0;
            if (frac > 1) frac = 1;

            int fillWidth = (int)(_ultBar.Width * frac);
            if (fillWidth <= 0) return;

            Color gold = Color.FromArgb(255, 200, 90);
            using (var brush = new LinearGradientBrush(
                new Rectangle(0, 0, fillWidth, _ultBar.Height),
                gold, ControlPaint.Dark(gold, 0.2f),
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, 0, 0, fillWidth, _ultBar.Height);
            }
        }
    }
}
