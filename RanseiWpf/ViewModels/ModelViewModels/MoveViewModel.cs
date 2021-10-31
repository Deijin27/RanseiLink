using Core;
using Core.Enums;
using Core.Models.Interfaces;
using System.Linq;
using System.Windows.Input;

namespace RanseiWpf.ViewModels
{
    public enum MoveAnimationPreviewMode
    {
        Startup,
        Projectile,
        Impact,
    }

    public class MoveViewModel : ViewModelBase, IViewModelForModel<IMove>
    {
        public MoveViewModel()
        {
            SetPreviewAnimationModeCommand = new RelayCommand<MoveAnimationPreviewMode>(mode =>
            {
                PreviewAnimationMode = mode;
                UpdatePreviewAnimation();
            });
        }

        private IMove _model;
        public IMove Model
        {
            get => _model;
            set
            {
                _model = value;
                UpdatePreviewAnimation();
            }
        }

        public string Name
        {
            get => Model.Name;
            set => RaiseAndSetIfChanged(Model.Name, value, v => Model.Name = v);
        }

        public bool MovementFlag_MovementOrKnockback
        {
            get => (Model.MovementFlags & MoveMovementFlags.MovementOrKnockback) != 0;
            set => RaiseAndSetIfChanged(MovementFlag_MovementOrKnockback, value, v => Model.MovementFlags ^= MoveMovementFlags.MovementOrKnockback);
        }

        public bool MovementFlag_InvertMovementDirection
        {
            get => (Model.MovementFlags & MoveMovementFlags.InvertMovementDirection) != 0;
            set => RaiseAndSetIfChanged(MovementFlag_InvertMovementDirection, value, v => Model.MovementFlags ^= MoveMovementFlags.InvertMovementDirection);
        }

        public bool MovementFlag_DoubleMovementDistance
        {
            get => (Model.MovementFlags & MoveMovementFlags.DoubleMovementDistance) != 0;
            set => RaiseAndSetIfChanged(MovementFlag_DoubleMovementDistance, value, v => Model.MovementFlags ^= MoveMovementFlags.DoubleMovementDistance);
        }

        public TypeId[] TypeItems { get; } = EnumUtil.GetValues<TypeId>().ToArray();
        public TypeId Type
        {
            get => Model.Type;
            set => RaiseAndSetIfChanged(Model.Type, value, v => Model.Type = v);
        }

        public uint Power
        {
            get => Model.Power;
            set => RaiseAndSetIfChanged(Model.Power, value, v => Model.Power = v);
        }

        public uint Accuracy
        {
            get => Model.Accuracy;
            set => RaiseAndSetIfChanged(Model.Accuracy, value, v => Model.Accuracy = v);
        }

        public MoveRangeId[] RangeItems { get; } = EnumUtil.GetValues<MoveRangeId>().ToArray();
        public MoveRangeId Range
        {
            get => Model.Range;
            set => RaiseAndSetIfChanged(Model.Range, value, v => Model.Range = v);
        }

        public MoveEffectId[] EffectItems { get; } = EnumUtil.GetValues<MoveEffectId>().ToArray();

        public MoveEffectId Effect1
        {
            get => Model.Effect1;
            set => RaiseAndSetIfChanged(Model.Effect1, value, v => Model.Effect1 = v);
        }

        public uint Effect1Chance
        {
            get => Model.Effect1Chance;
            set => RaiseAndSetIfChanged(Model.Effect1Chance, value, v => Model.Effect1Chance = v);
        }

        public MoveEffectId Effect2
        {
            get => Model.Effect2;
            set => RaiseAndSetIfChanged(Model.Effect2, value, v => Model.Effect2 = v);
        }

        public uint Effect2Chance
        {
            get => Model.Effect2Chance;
            set => RaiseAndSetIfChanged(Model.Effect2Chance, value, v => Model.Effect2Chance = v);
        }

        public MoveAnimationId[] AnimationItems { get; } = EnumUtil.GetValues<MoveAnimationId>().ToArray();

        public MoveAnimationId StartupAnimation
        {
            get => Model.StartupAnimation;
            set
            {
                if (RaiseAndSetIfChanged(Model.StartupAnimation, value, v => Model.StartupAnimation = v))
                {
                    UpdatePreviewAnimation();
                }
            }
        }

        public MoveAnimationId ProjectileAnimation
        {
            get => Model.ProjectileAnimation;
            set
            {
                if (RaiseAndSetIfChanged(Model.ProjectileAnimation, value, v => Model.ProjectileAnimation = v))
                {
                    UpdatePreviewAnimation();
                }
            }
        }

        public MoveAnimationId ImpactAnimation
        {
            get => Model.ImpactAnimation;
            set
            {
                if (RaiseAndSetIfChanged(Model.ImpactAnimation, value, v => Model.ImpactAnimation = v))
                {
                    UpdatePreviewAnimation();
                }
            }
        }

        public MoveAnimationTargetId[] AnimationTargetItems { get; } = EnumUtil.GetValues<MoveAnimationTargetId>().ToArray();

        public MoveAnimationTargetId AnimationTarget1
        {
            get => Model.AnimationTarget1;
            set => RaiseAndSetIfChanged(Model.AnimationTarget1, value, v => Model.AnimationTarget1 = v);
        }

        public MoveAnimationTargetId AnimationTarget2
        {
            get => Model.AnimationTarget2;
            set => RaiseAndSetIfChanged(Model.AnimationTarget2, value, v => Model.AnimationTarget2 = v);
        }

        public MoveMovementAnimationId[] MovementAnimationItems { get; } = EnumUtil.GetValues<MoveMovementAnimationId>().ToArray();

        public MoveMovementAnimationId MovementAnimation
        {
            get => Model.MovementAnimation;
            set => RaiseAndSetIfChanged(Model.MovementAnimation, value, v => Model.MovementAnimation = v);
        }

        private string _currentPreviewAnimationUri;
        public string CurrentPreviewAnimationUri
        {
            get => _currentPreviewAnimationUri;
            set => RaiseAndSetIfChanged(ref _currentPreviewAnimationUri, value);
        }

        private string _currentPreviewAnimationName;
        public string CurrentPreviewAnimationName
        {
            get => _currentPreviewAnimationName;
            set => RaiseAndSetIfChanged(ref _currentPreviewAnimationName, value);
        }

        private MoveAnimationPreviewMode PreviewAnimationMode { get; set; } = MoveAnimationPreviewMode.Startup;

        public ICommand SetPreviewAnimationModeCommand { get; }

        private void UpdatePreviewAnimation()
        {
            switch (PreviewAnimationMode)
            {
                case MoveAnimationPreviewMode.Startup:
                    CurrentPreviewAnimationUri = GetAnimationUri(StartupAnimation);
                    CurrentPreviewAnimationName = StartupAnimation.ToString();
                    break;
                case MoveAnimationPreviewMode.Projectile:
                    CurrentPreviewAnimationUri = GetAnimationUri(ProjectileAnimation);
                    CurrentPreviewAnimationName = ProjectileAnimation.ToString();
                    break;
                case MoveAnimationPreviewMode.Impact:
                    CurrentPreviewAnimationUri = GetAnimationUri(ImpactAnimation);
                    CurrentPreviewAnimationName = ImpactAnimation.ToString();
                    break;
            };
        }

        private string GetAnimationUri(MoveAnimationId id)
        {
            string uri = id switch
            {
                MoveAnimationId.WaterDropletSplash => "https://media2.giphy.com/media/iUYeOIoi5AyxCeqScu/giphy.gif",
                MoveAnimationId.OrangeOrbBurst => "https://media1.giphy.com/media/4HTfc17O2Nug1gfLvw/giphy.gif",
                MoveAnimationId.DoubleOrangeOrbBurst => "https://media4.giphy.com/media/lZHEYKCD0W7GEjRvmV/giphy.gif",
                MoveAnimationId.OrangeOrbScratch => "https://media1.giphy.com/media/bsTxxxv8uB8nIfFUVk/giphy.gif",
                _ => "https://media0.giphy.com/media/xFtpI1mGHZvLV5Sqf3/giphy.gif"
            };

            return uri;
        }
    }
}
