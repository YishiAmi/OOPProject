using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using RpgLibrary.Combat;
using RpgLibrary.Contracts;
using RpgLibrary.Exceptions;
using RPGGameLibrary.Items;

namespace RpgWinForm
{
    public class MainForm : Form
    {
        // --- Game state 
        private Hero _player = null!;
        private ICombatant _enemy = null!;
        private AttackSkill _slash = null!;
        private HealSkill _mend = null!;
        private Weapon _sword = null!;
        private Armor _armor = null!;
        private Potion _potionTemplate = null!;
        private readonly Random _rng = new Random();

        // --- UI controls 
        private Label _playerName = null!;
        private Label _playerStats = null!;
        private ProgressBar _playerHpBar = null!;
        private Label _playerHpLabel = null!;

        private Label _enemyName = null!;
        private Label _enemyStats = null!;
        private ProgressBar _enemyHpBar = null!;
        private Label _enemyHpLabel = null!;
        private Label _bossChargeLabel = null!;
        private ProgressBar _bossChargeBar = null!;

        private TextBox _logBox = null!;

        private Button _btnAttack = null!;
        private Button _btnHeal = null!;
        private Button _btnPotion = null!;
        private Button _btnTryUltimate = null!;
        private Button _btnNextEnemy = null!;
        private Button _btnFightBoss = null!;
        private Button _btnPartyBattle = null!;
        private Button _btnBattleArena = null!;
        private Button _btnLibraryTour = null!;

        public MainForm()
        {
            Text = "RpgLibrary Demo";
            Size = new Size(1400, 650);
            MinimumSize = new Size(1350, 600);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 245, 250);
            Font = new Font("Segoe UI", 9F);

            BuildUi();

            // Redirect Console output so library messages appear in the log.
            Console.SetOut(new TextBoxWriter(_logBox));

            NewGame();
        }

        private void BuildUi()
        {
            // Menu bar
            var menu = new MenuStrip();
            var fileMenu = new ToolStripMenuItem("Game");
            var newGameItem = new ToolStripMenuItem("New Game", null, (_, _) => NewGame());
            var exitItem = new ToolStripMenuItem("Exit", null, (_, _) => Close());
            fileMenu.DropDownItems.Add(newGameItem);
            fileMenu.DropDownItems.Add(new ToolStripSeparator());
            fileMenu.DropDownItems.Add(exitItem);
            menu.Items.Add(fileMenu);
            MainMenuStrip = menu;
            Controls.Add(menu);

            // Root layout: 2 rows (game area, action bar)
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1,
                Padding = new Padding(10),
            };
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            Controls.Add(root);
            root.BringToFront();

            // Top: 3-column layout (Player | Log | Enemy)
            var top = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 1,
                ColumnCount = 3,
            };
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            root.Controls.Add(top, 0, 0);

            top.Controls.Add(BuildPlayerPanel(), 0, 0);
            top.Controls.Add(BuildLogPanel(), 1, 0);
            top.Controls.Add(BuildEnemyPanel(), 2, 0);

            // Bottom: action buttons
            root.Controls.Add(BuildActionBar(), 0, 1);
        }

        private GroupBox BuildPlayerPanel()
        {
            var group = new GroupBox
            {
                Text = "Player",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 100),
                Padding = new Padding(10),
            };

            _playerName = new Label
            {
                Text = "-",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                AutoSize = false,
                Height = 30,
                Dock = DockStyle.Top,
                ForeColor = Color.FromArgb(80, 40, 140),
            };

            _playerHpLabel = new Label { Text = "HP: -/-", Dock = DockStyle.Top, Height = 20, Font = new Font("Segoe UI", 9F) };
            _playerHpBar = new ProgressBar
            {
                Dock = DockStyle.Top,
                Height = 20,
                ForeColor = Color.LimeGreen,
                Style = ProgressBarStyle.Continuous,
            };

            _playerStats = new Label
            {
                Text = "",
                Dock = DockStyle.Top,
                Height = 100,
                Font = new Font("Segoe UI", 9F),
                Padding = new Padding(0, 10, 0, 0),
            };

            group.Controls.Add(_playerStats);
            group.Controls.Add(_playerHpBar);
            group.Controls.Add(_playerHpLabel);
            group.Controls.Add(_playerName);
            return group;
        }

        private GroupBox BuildEnemyPanel()
        {
            var group = new GroupBox
            {
                Text = "Enemy",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(140, 40, 40),
                Padding = new Padding(10),
            };

            _enemyName = new Label
            {
                Text = "-",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                AutoSize = false,
                Height = 30,
                Dock = DockStyle.Top,
                ForeColor = Color.FromArgb(160, 40, 40),
            };

            _enemyHpLabel = new Label { Text = "HP: -/-", Dock = DockStyle.Top, Height = 20 };
            _enemyHpBar = new ProgressBar
            {
                Dock = DockStyle.Top,
                Height = 20,
                Style = ProgressBarStyle.Continuous,
                ForeColor = Color.Firebrick,
            };

            _enemyStats = new Label
            {
                Text = "",
                Dock = DockStyle.Top,
                Height = 60,
                Padding = new Padding(0, 10, 0, 0),
            };

            _bossChargeLabel = new Label
            {
                Text = "",
                Dock = DockStyle.Top,
                Height = 20,
                ForeColor = Color.DarkOrange,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Visible = false,
            };
            _bossChargeBar = new ProgressBar
            {
                Dock = DockStyle.Top,
                Height = 15,
                Style = ProgressBarStyle.Continuous,
                ForeColor = Color.DarkOrange,
                Visible = false,
            };

            group.Controls.Add(_bossChargeBar);
            group.Controls.Add(_bossChargeLabel);
            group.Controls.Add(_enemyStats);
            group.Controls.Add(_enemyHpBar);
            group.Controls.Add(_enemyHpLabel);
            group.Controls.Add(_enemyName);
            return group;
        }

        private GroupBox BuildLogPanel()
        {
            var group = new GroupBox
            {
                Text = "Battle Log",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60),
                Padding = new Padding(10),
            };

            _logBox = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                Dock = DockStyle.Fill,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 9F),
                BackColor = Color.FromArgb(30, 30, 40),
                ForeColor = Color.FromArgb(220, 220, 220),
                BorderStyle = BorderStyle.None,
                WordWrap = true,
            };

            group.Controls.Add(_logBox);
            return group;
        }

        private Panel BuildActionBar()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(230, 230, 240) };

            _btnAttack = MakeButton("Attack (Slash)", 10);
            _btnAttack.Click += (_, _) => DoPlayerAction(() => _slash.Use(_player, _enemy));

            _btnHeal = MakeButton("Heal (Field Bandage)", 150);
            _btnHeal.Click += (_, _) => DoPlayerAction(() => _mend.Use(_player, _player));

            _btnPotion = MakeButton("Use Potion", 300);
            _btnPotion.Click += (_, _) => DoPlayerAction(UsePotion);

            _btnTryUltimate = MakeButton("Force Boss Ultimate", 420);
            _btnTryUltimate.Click += (_, _) => TryForceBossUltimate();
            _btnTryUltimate.BackColor = Color.FromArgb(255, 220, 180);

            _btnNextEnemy = MakeButton("Next Enemy", 580);
            _btnNextEnemy.Click += (_, _) => { SpawnRandomEnemy(); UpdateUi(); };

            _btnFightBoss = MakeButton("Fight Boss", 690);
            _btnFightBoss.Click += (_, _) => { SpawnBoss(); UpdateUi(); };
            _btnFightBoss.BackColor = Color.FromArgb(255, 200, 200);

            _btnPartyBattle = MakeButton("Party Battle (Auto)", 820);
            _btnPartyBattle.Click += (_, _) => RunPartyBattle();
            _btnPartyBattle.BackColor = Color.FromArgb(200, 220, 255);

            _btnBattleArena = MakeButton("Battle Arena", 960);
            _btnBattleArena.Click += (_, _) => OpenBattleArena();
            _btnBattleArena.BackColor = Color.FromArgb(30, 34, 60);
            _btnBattleArena.ForeColor = Color.FromArgb(255, 200, 90);
            _btnBattleArena.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            _btnLibraryTour = MakeButton("Library Tour", 1100);
            _btnLibraryTour.Click += (_, _) => new DemoForm().Show(this);
            _btnLibraryTour.BackColor = Color.FromArgb(30, 34, 60);
            _btnLibraryTour.ForeColor = Color.FromArgb(120, 210, 255);
            _btnLibraryTour.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            panel.Controls.AddRange(new Control[]
            {
                _btnAttack, _btnHeal, _btnPotion, _btnTryUltimate,
                _btnNextEnemy, _btnFightBoss, _btnPartyBattle, _btnBattleArena,
                _btnLibraryTour
            });

            return panel;
        }

        // opens the interactive Battle Arena form
        private void OpenBattleArena()
        {
            var companion = new Hero("Kael", 80, 10, 5, speed: 11);
            var ariaSkills = new System.Collections.Generic.List<Skill>
            {
                new AttackSkill("Slash", 18),
                new HealSkill("Field Bandage", 15, TargetType.SingleAlly),
            };
            var kaelSkills = new System.Collections.Generic.List<Skill>
            {
                new AttackSkill("Power Strike", 22),
            };
            var party = new System.Collections.Generic.List<PartyMember>
            {
                new PartyMember(_player, ariaSkills, ultimate: new DrainUltimate("Blood Draw")),
                new PartyMember(companion, kaelSkills, ultimate: new HealUltimate("Second Wind")),
            };
            var foes = new System.Collections.Generic.List<ICombatant>
            {
                EnemyFactory.CreateWeakEnemy("Goblin Scout"),
                EnemyFactory.CreateHealerEnemy("Forest Shaman"),
                EnemyFactory.CreateStrongEnemy("Forest Ogre"),
            };

            var arena = new BattleForm(party, foes);
            arena.Show(this);
        }

        // runs a party battle via AutoBattleUI, log goes to _logBox
        private void RunPartyBattle()
        {
            _logBox.Clear();
            Console.WriteLine("========== PARTY TURN-BASED BATTLE ==========\n");

            // Party: the player + a companion
            var companion = new Hero("Kael", 80, 10, 5, speed: 11);
            var ariaSkills = new System.Collections.Generic.List<Skill>
            {
                new AttackSkill("Slash", 18),
                new HealSkill("Field Bandage", 15, TargetType.SingleAlly)
            };
            var kaelSkills = new System.Collections.Generic.List<Skill>
            {
                new AttackSkill("Power Strike", 22)
            };
            var party = new System.Collections.Generic.List<PartyMember>
            {
                new PartyMember(_player, ariaSkills, ultimate: new DrainUltimate("Blood Draw")),
                new PartyMember(companion, kaelSkills, ultimate: new HealUltimate("Second Wind"))
            };

            // Enemy party of three
            var foes = new System.Collections.Generic.List<ICombatant>
            {
                EnemyFactory.CreateWeakEnemy("Goblin Scout"),
                EnemyFactory.CreateHealerEnemy("Forest Shaman"),
                EnemyFactory.CreateStrongEnemy("Forest Ogre"),
            };

            var battle = new BattleSystem(
                party, foes,
                ui: new AutoBattleUI(),
                enemyStrategy: new RandomTargetStrategy(),
                settings: new BattleSettings { UltimateChargePerTurn = 1, MaxRounds = 30 });

            battle.Run();

            UpdateUi();
            CheckOutcome();
        }

        private Button MakeButton(string text, int x)
        {
            return new Button
            {
                Text = text,
                Location = new Point(x, 15),
                Size = new Size(130, 40),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                FlatStyle = FlatStyle.System,
            };
        }

        private void NewGame()
        {
            _logBox.Clear();
            Console.WriteLine("========== NEW GAME ==========");

            _player = new Hero
            {
                Name = "Aria",
                Level = 1,
                MaxHealth = 100,
                Health = 100,
                Attack = 12,
                Defense = 8,
                Speed = 10,
                Description = "Wandering swordswoman."
            };

            _sword = new Weapon("Training Sword", "Basic blade.", 25, 5, "Sword");
            _armor = new Armor("Leather Armor", "Light chest.", 30, 4, "Light");
            _player.EquipWeapon(_sword);
            _player.EquipArmor(_armor);

            _potionTemplate = new Potion("Small Potion", "Restores 20 HP.", 10, 20);
            // Give the player 3 potions
            for (int i = 0; i < 3; i++)
            {
                try { _player.Inventory.AddItem(_potionTemplate); }
                catch (Exception ex) { Console.WriteLine($"[Error] {ex.Message}"); }
            }

            _slash = new AttackSkill("Slash", 18);
            _mend = new HealSkill("Field Bandage", 15);

            SpawnRandomEnemy();
            UpdateUi();
        }

        private void SpawnRandomEnemy()
        {
            int roll = _rng.Next(3);
            _enemy = roll switch
            {
                0 => EnemyFactory.CreateWeakEnemy("Goblin Scout"),
                1 => EnemyFactory.CreateStrongEnemy("Forest Ogre"),
                _ => EnemyFactory.CreateHealerEnemy("Forest Shaman"),
            };
            Console.WriteLine($"\n--- A wild {_enemy.Name} appears! ---\n");
        }

        private void SpawnBoss()
        {
            _enemy = _rng.Next(2) == 0
                ? EnemyFactory.CreateDamageBoss("Forest Warden")
                : EnemyFactory.CreateDrainBoss("Shadow Wraith");
            Console.WriteLine($"\n=== BOSS FIGHT: {_enemy.Name} appears! ===\n");
        }

        private void DoPlayerAction(Action action)
        {
            if (!_player.IsAlive() || !_enemy.IsAlive()) return;

            try
            {
                action();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] {ex.Message}");
            }

            // Enemy retaliates if still alive
            if (_enemy.IsAlive() && _enemy is Enemy enemyObj)
            {
                enemyObj.TakeTurn(_player);
            }

            UpdateUi();
            CheckOutcome();
        }

        private void UsePotion()
        {
            var potion = _player.Inventory.Items.OfType<Potion>().FirstOrDefault();
            if (potion == null)
            {
                Console.WriteLine("No potions left!");
                return;
            }

            potion.Drink();
            _player.Heal(potion.HealAmount);
            _player.Inventory.RemoveItem(potion);
        }

        private void TryForceBossUltimate()
        {
            if (_enemy is not Boss boss)
            {
                Console.WriteLine("Only bosses have ultimates. Click 'Fight Boss' first.");
                return;
            }

            try
            {
                boss.Ultimate.Use(boss, _player);
            }
            catch (UltimateNotChargedException ex)
            {
                Console.WriteLine($"[Caught UltimateNotChargedException] {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] {ex.Message}");
            }

            UpdateUi();
            CheckOutcome();
        }

        private void UpdateUi()
        {
            // Player
            _playerName.Text = _player.Name;
            _playerHpBar.Maximum = Math.Max(1, _player.MaxHealth);
            _playerHpBar.Value = Math.Max(0, Math.Min(_player.Health, _player.MaxHealth));
            _playerHpLabel.Text = $"HP: {_player.Health}/{_player.MaxHealth}";

            int potionCount = _player.Inventory.Items.OfType<Potion>().Count();
            _playerStats.Text =
                $"Level: {_player.Level}\n" +
                $"Attack: {_player.Attack}\n" +
                $"Defense: {_player.Defense}\n" +
                $"Speed: {_player.Speed}\n" +
                $"Gold: {_player.Gold}\n" +
                $"Exp: {_player.Experience}/{_player.ExperienceToNextLevel}\n" +
                $"Potions: {potionCount}";

            _btnPotion.Enabled = potionCount > 0 && _player.IsAlive();

            // Enemy
            _enemyName.Text = _enemy.Name;
            _enemyHpBar.Maximum = Math.Max(1, _enemy.MaxHealth);
            _enemyHpBar.Value = Math.Max(0, Math.Min(_enemy.Health, _enemy.MaxHealth));
            _enemyHpLabel.Text = $"HP: {_enemy.Health}/{_enemy.MaxHealth}";

            string enemyType = _enemy switch
            {
                Boss => "BOSS",
                WeakEnemy => "Weak Enemy",
                StrongEnemy => "Strong Enemy",
                HealerEnemy => "Healer Enemy",
                _ => _enemy.GetType().Name,
            };
            _enemyStats.Text =
                $"Type: {enemyType}\n" +
                $"Defense: {_enemy.Defense}";

            // Boss ultimate charge display
            if (_enemy is Boss bossEnemy)
            {
                _bossChargeLabel.Visible = true;
                _bossChargeBar.Visible = true;
                _bossChargeLabel.Text = $"Ultimate charge: {bossEnemy.Ultimate.CurrentCharge}/{bossEnemy.Ultimate.MaxCharge}";
                _bossChargeBar.Maximum = Math.Max(1, bossEnemy.Ultimate.MaxCharge);
                _bossChargeBar.Value = Math.Max(0, Math.Min(bossEnemy.Ultimate.CurrentCharge, bossEnemy.Ultimate.MaxCharge));
                _btnTryUltimate.Enabled = true;
            }
            else
            {
                _bossChargeLabel.Visible = false;
                _bossChargeBar.Visible = false;
                _btnTryUltimate.Enabled = false;
            }

            bool inCombat = _player.IsAlive() && _enemy.IsAlive();
            _btnAttack.Enabled = inCombat;
            _btnHeal.Enabled = inCombat;
        }

        private void CheckOutcome()
        {
            if (!_player.IsAlive())
            {
                Console.WriteLine("\n*** YOU DIED ***\n");
                MessageBox.Show("You died! Start a new game from the Game menu.",
                    "Defeat", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                _btnAttack.Enabled = false;
                _btnHeal.Enabled = false;
                _btnPotion.Enabled = false;
                return;
            }

            if (!_enemy.IsAlive())
            {
                int gold = _enemy is Boss ? 100 : 20;
                int exp = _enemy is Boss ? 200 : 50;
                _player.EarnGold(gold);
                _player.GainExperience(exp);

                Console.WriteLine($"\n*** Victory! +{gold} gold, +{exp} exp ***\n");
                UpdateUi();

                MessageBox.Show($"You defeated {_enemy.Name}!\n\n+{gold} gold\n+{exp} exp",
                    "Victory", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
