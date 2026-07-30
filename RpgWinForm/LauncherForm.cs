using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using RpgLibrary.Combat;
using RpgLibrary.Contracts;

namespace RpgWinForm
{
    public class LauncherForm : Form
    {
        public LauncherForm()
        {
            Text = "RPG Library";
            Size = new Size(700, 460);
            MinimumSize = new Size(660, 420);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(20, 22, 40);
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 10F);

            BuildUi();
        }

        private void BuildUi()
        {
            var title = new Label
            {
                Text = "RPG LIBRARY",
                Dock = DockStyle.Top,
                Height = 80,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 200, 90),
            };
            Controls.Add(title);

            var footer = new Label
            {
                Text = "Pick one to begin.",
                Dock = DockStyle.Bottom,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
            };
            Controls.Add(footer);

            // Two cards, side by side
            var cards = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 1,
                ColumnCount = 2,
                Padding = new Padding(20),
            };
            cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            Controls.Add(cards);

            cards.Controls.Add(
                MakeCard("Battle Arena",
                         "Party turn-based fight.\nAria + Kael vs the woods.",
                         Color.FromArgb(255, 110, 120),
                         OpenBattleArena),
                0, 0);

            cards.Controls.Add(
                MakeCard("Library Tour",
                         "Step through every library\nAPI, one button at a time.",
                         Color.FromArgb(120, 210, 255),
                         OpenLibraryTour),
                1, 0);
        }

        private Panel MakeCard(string title, string subtitle, Color accent, EventHandler onClick)
        {
            var card = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(10),
                BackColor = Color.FromArgb(30, 34, 60),
                Cursor = Cursors.Hand,
            };

            var titleLabel = new Label
            {
                Text = title,
                Dock = DockStyle.Top,
                Height = 60,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = accent,
            };

            var subtitleLabel = new Label
            {
                Text = subtitle,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.White,
            };

            var btn = new Button
            {
                Text = "Launch",
                Dock = DockStyle.Bottom,
                Height = 45,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                BackColor = accent,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += onClick;

            // clicking anywhere on the card works
            card.Click += onClick;
            titleLabel.Click += onClick;
            subtitleLabel.Click += onClick;

            card.Controls.Add(subtitleLabel);
            card.Controls.Add(titleLabel);
            card.Controls.Add(btn);
            return card;
        }

        private void OpenBattleArena(object? sender, EventArgs e)
        {
            var party = BuildParty();
            var foes = new List<ICombatant>
            {
                EnemyFactory.CreateWeakEnemy("Goblin Scout"),
                EnemyFactory.CreateHealerEnemy("Forest Shaman"),
                EnemyFactory.CreateStrongEnemy("Forest Ogre"),
            };
            new BattleForm(party, foes).Show(this);
        }

        private void OpenLibraryTour(object? sender, EventArgs e)
        {
            new DemoForm().Show(this);
        }

        private static List<PartyMember> BuildParty()
        {
            var aria = new Hero("Aria", 100, 12, 8, speed: 10);
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
                new PartyMember(aria, ariaSkills, new DrainUltimate("Blood Draw")),
                new PartyMember(kael, kaelSkills, new HealUltimate("Second Wind")),
            };
        }
    }
}
