using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using RpgLibrary;
using RpgLibrary.Combat;
using RpgLibrary.Contracts;
using RpgLibrary.World;
using RPGGameLibrary.Items;

namespace RpgWinForm
{
    // Click-through walkthrough of the library. Each button hits ONE API
    // and shows the result in the log. State panel refreshes after each click.
    public class DemoForm : Form
    {
        private Character? _aria;
        private Map? _map;
        private GameManager? _game;
        private NPC? _villager;
        private MainQuest? _quest;
        private Shop? _shop;
        private ShopSlot? _potionSlot;
        private Potion? _smallPotion;
        private Weapon? _trainingSword;
        private Armor? _leatherArmor;

        private Label _stateHeader = null!;
        private Label _statePlayer = null!;
        private Label _stateQuest  = null!;
        private Label _stateShop   = null!;
        private TextBox _logBox    = null!;

        public DemoForm()
        {
            Text = "RPG Library Walkthrough";
            Size = new Size(1100, 700);
            MinimumSize = new Size(1000, 620);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(20, 22, 40);
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 10F);

            BuildUi();

            // send Console.WriteLine into the log
            Console.SetOut(new TextBoxWriter(_logBox));

            AppendLog("Ready. Click [Setup] to build the world.");
        }

        private void BuildUi()
        {
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1,
                Padding = new Padding(12),
            };
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 180));
            Controls.Add(root);

            var top = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 1,
                ColumnCount = 2,
            };
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));
            root.Controls.Add(top, 0, 0);

            top.Controls.Add(BuildStatePanel(), 0, 0);
            top.Controls.Add(BuildLogPanel(),   1, 0);

            root.Controls.Add(BuildActionBar(), 0, 1);
        }

        private GroupBox BuildStatePanel()
        {
            var group = new GroupBox
            {
                Text = "WORLD STATE",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(120, 210, 255),
                Padding = new Padding(10),
            };

            _stateHeader = MakeLabel("(not set up yet)", Color.FromArgb(255, 200, 90), 14F, FontStyle.Bold);
            _statePlayer = MakeLabel("Player: -", Color.White, 10F, FontStyle.Regular);
            _stateQuest  = MakeLabel("Quest: -",  Color.White, 10F, FontStyle.Regular);
            _stateShop   = MakeLabel("Shop: -",   Color.White, 10F, FontStyle.Regular);

            var stack = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(8),
            };
            stack.Controls.Add(_stateHeader);
            stack.Controls.Add(_statePlayer);
            stack.Controls.Add(_stateQuest);
            stack.Controls.Add(_stateShop);
            group.Controls.Add(stack);
            return group;
        }

        private Label MakeLabel(string text, Color color, float size, FontStyle style)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Margin = new Padding(0, 4, 0, 8),
                ForeColor = color,
                Font = new Font("Segoe UI", size, style),
                MaximumSize = new Size(280, 0),
            };
        }

        private GroupBox BuildLogPanel()
        {
            var group = new GroupBox
            {
                Text = "LOG",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 200, 90),
                Padding = new Padding(10),
            };
            _logBox = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                Dock = DockStyle.Fill,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 9.5F),
                BackColor = Color.FromArgb(15, 17, 30),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None,
            };
            group.Controls.Add(_logBox);
            return group;
        }

        private Panel BuildActionBar()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(30, 34, 60),
                Padding = new Padding(10),
            };

            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoScroll = true,
            };

            flow.Controls.Add(MakeButton("1. Setup World",        Color.FromArgb(60, 90, 130),  OnSetup));
            flow.Controls.Add(MakeButton("2. Talk to NPC",        Color.FromArgb(60, 110, 130), OnTalk));
            flow.Controls.Add(MakeButton("3. Accept Quest",       Color.FromArgb(60, 110, 130), OnAcceptQuest));
            flow.Controls.Add(MakeButton("4. Add Quest Progress", Color.FromArgb(60, 110, 130), OnAddProgress));
            flow.Controls.Add(MakeButton("5. Buy Potion",         Color.FromArgb(70, 130, 90),  OnBuy));
            flow.Controls.Add(MakeButton("6. Skirmish",           Color.FromArgb(140, 90, 40),  OnSkirmish));
            flow.Controls.Add(MakeButton("7. Boss Fight",         Color.FromArgb(160, 60, 60),  OnBossFight));
            flow.Controls.Add(MakeButton("8. Complete Quest",     Color.FromArgb(140, 100, 40), OnCompleteQuest));

            panel.Controls.Add(flow);
            return panel;
        }

        private Button MakeButton(string text, Color color, EventHandler onClick)
        {
            var btn = new Button
            {
                Text = text,
                Size = new Size(170, 45),
                BackColor = color,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(4),
            };
            btn.Click += onClick;
            return btn;
        }

        private void OnSetup(object? sender, EventArgs e)
        {
            AppendLog("\n--- [1] Setup ---");

            _map = new Map("Whispering Forest", difficulty: 2);
            AppendLog($"  Map: {_map.MapName} (difficulty {_map.Difficulty})");

            _trainingSword = new Weapon("Training Sword", "A basic starter blade.", 25, 5, "Sword");
            _leatherArmor  = new Armor("Leather Armor", "Light chest piece.", 30, 4, "Light");
            _smallPotion   = new Potion("Small Potion", "Restores 20 HP.", 10, 20);
            AppendLog("  Created 3 items");

            _aria = new Character
            {
                Name = "Aria", Level = 1, MaxHealth = 100, Health = 100,
                Attack = 12, Defense = 8, Speed = 10,
                Description = "Wandering swordswoman."
            };
            _aria.EquipWeapon(_trainingSword);
            _aria.EquipArmor(_leatherArmor);
            _aria.Inventory.AddItem(_smallPotion);
            AppendLog($"  Character: {_aria.Name} (ATK {_aria.Attack}, DEF {_aria.Defense})");

            _game = new GameManager(_aria, _map, startingGold: 50);
            AppendLog($"  GameManager wired (gold: {_game.PlayerGold})");

            _quest = new MainQuest("First Steps", "Defeat the forest boss.", 100, chapter: 1);
            _villager = new NPC("Old Ben", "Beware the boss in the woods.", offeredQuest: _quest);
            _map.AddNpc(_villager);
            AppendLog($"  NPC: {_villager.Name} offering quest \"{_quest.Title}\"");

            _shop = new Shop("Ben's Wares");
            _potionSlot = _shop.AddStock(_smallPotion, quantity: 5);
            _map.SetLocalShop(_shop);
            AppendLog($"  Shop: {_shop.ShopName}");

            RefreshState();
        }

        private void OnTalk(object? sender, EventArgs e)
        {
            if (!EnsureSetup()) return;
            AppendLog("\n--- [2] Talk to NPC ---");

            var r = _game!.TalkToNpc(_villager!);
            AppendLog($"  Status: {r.Status}");
            AppendLog($"  {r.Npc.Name}: \"{r.Dialogue}\"");
            if (r.CanAcceptQuest && r.OfferedQuest != null)
                AppendLog($"  offers quest: {r.OfferedQuest.Title}");
        }

        private void OnAcceptQuest(object? sender, EventArgs e)
        {
            if (!EnsureSetup()) return;
            AppendLog("\n--- [3] Accept Quest ---");

            var r = _game!.AcceptQuest(_villager!);
            AppendLog(r.Succeeded
                ? $"  Accepted: {r.Quest!.Title}"
                : $"  Failed: {r.Status}");

            RefreshState();
        }

        private void OnAddProgress(object? sender, EventArgs e)
        {
            if (!EnsureSetup()) return;
            AppendLog("\n--- [4] Add Progress ---");

            var r = _game!.AddQuestProgress(_quest!, amount: 1);
            AppendLog(r.Succeeded
                ? $"  Progress now {_quest!.CurrentProgress}/{_quest.RequiredProgress}"
                : $"  Failed: {r.Status}");

            RefreshState();
        }

        private void OnBuy(object? sender, EventArgs e)
        {
            if (!EnsureSetup()) return;
            AppendLog("\n--- [5] Buy Potion ---");

            var r = _game!.BuyItem(_potionSlot!);
            AppendLog(r.Succeeded
                ? $"  Bought {r.Item.Name} for {r.PricePaid}g. Remaining: {r.GoldRemaining}g"
                : $"  Failed: {r.Status}. Gold: {r.GoldRemaining}g");

            RefreshState();
        }

        private void OnSkirmish(object? sender, EventArgs e)
        {
            if (!EnsureSetup()) return;
            AppendLog("\n--- [6] Skirmish ---");

            var foes = new List<ICombatant>
            {
                EnemyFactory.CreateWeakEnemy("Goblin Scout"),
                EnemyFactory.CreateHealerEnemy("Forest Shaman"),
            };
            new BattleForm(BuildParty(), foes).Show(this);
        }

        private void OnBossFight(object? sender, EventArgs e)
        {
            if (!EnsureSetup()) return;
            AppendLog("\n--- [7] Boss Fight ---");

            var foes = new List<ICombatant>
            {
                EnemyFactory.CreateDamageBoss("Forest Warden"),
            };
            new BattleForm(BuildParty(), foes).Show(this);
        }

        private void OnCompleteQuest(object? sender, EventArgs e)
        {
            if (!EnsureSetup()) return;
            AppendLog("\n--- [8] Complete Quest ---");

            var r = _game!.CompleteQuest(_quest!);
            AppendLog(r.Succeeded
                ? $"  Complete! Awarded {r.GoldAwarded}g. Total: {r.GoldRemaining}g"
                : $"  Failed: {r.Status}");

            RefreshState();
        }

        private bool EnsureSetup()
        {
            if (_game != null) return true;
            MessageBox.Show("Click [1. Setup World] first.", "Setup Required",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }

        private List<PartyMember> BuildParty()
        {
            var kael = new Hero("Kael", 80, 10, 5, speed: 11);
            var ariaSkills = new List<Skill>
            {
                new AttackSkill("Slash", 18),
                new HealSkill("Field Bandage", 15, TargetType.SingleAlly),
            };
            var kaelSkills = new List<Skill>
            {
                new AttackSkill("Power Strike", 22),
            };
            return new List<PartyMember>
            {
                new PartyMember(_aria!, ariaSkills, new DrainUltimate("Blood Draw")),
                new PartyMember(kael,   kaelSkills, new HealUltimate("Second Wind")),
            };
        }

        private void RefreshState()
        {
            if (_aria == null || _game == null) return;

            _stateHeader.Text = $"{_aria.Name} - {_game.CurrentMap.MapName}";
            _statePlayer.Text =
                $"HP: {_aria.Health}/{_aria.MaxHealth}\n" +
                $"Level: {_aria.Level}\n" +
                $"ATK {_aria.Attack}  DEF {_aria.Defense}  SPD {_aria.Speed}\n" +
                $"Gold: {_game.PlayerGold}\n" +
                $"Potions: {CountPotions()}";

            if (_quest != null)
            {
                _stateQuest.Text =
                    $"Quest: {_quest.Title}\n" +
                    $"Status: {_quest.Status}\n" +
                    $"Progress: {_quest.CurrentProgress}/{_quest.RequiredProgress}\n" +
                    $"Reward: {_quest.GoldReward}g";
            }

            if (_shop != null)
            {
                int stock = _potionSlot != null ? _potionSlot.Quantity : 0;
                _stateShop.Text = $"Shop: {_shop.ShopName}\nPotions in stock: {stock}";
            }
        }

        private int CountPotions()
        {
            if (_aria == null) return 0;
            int n = 0;
            foreach (var i in _aria.Inventory.Items)
                if (i is Potion) n++;
            return n;
        }

        private void AppendLog(string message)
        {
            if (_logBox.IsDisposed) return;
            if (_logBox.InvokeRequired)
                _logBox.BeginInvoke(new Action(() => _logBox.AppendText(message + Environment.NewLine)));
            else
                _logBox.AppendText(message + Environment.NewLine);
        }
    }
}
