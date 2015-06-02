using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COG.Dredger.Entities;

namespace COG.Dredger
{
    public enum BehaviorStatus
    {
        Failed,
        Success,
        Running
    }

    #region Behavior
    public abstract class Behavior<T>
        where T : Character
    {
        public abstract BehaviorStatus Process(T character, float dt);
    }
    #endregion Behavior

    #region Composition
    public abstract class CompositionBehavior<T> : Behavior<T>, IEnumerable<Behavior<T>>
        where T : Character
    {
        protected List<Behavior<T>> m_nodes = new List<Behavior<T>>();
        public void AddNode(Behavior<T> node)
        {
            m_nodes.Add(node);
        }

        public IEnumerator<Behavior<T>> GetEnumerator()
        {
            return m_nodes.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return m_nodes.GetEnumerator();
        }
    }
    #endregion Composition

    #region Functor
    public class FuncBehavior<T> : Behavior<T>
        where T : Character
    {
        protected Func<T, float, BehaviorStatus> m_func;
        protected FuncBehavior(Func<T, float, BehaviorStatus> action)
        {
            m_func = action;
        }

        public override BehaviorStatus Process(T character, float dt)
        {
            return m_func(character, dt);
        }
    }
    #endregion Functor

    #region Action
    public class ActionBehavior<T> : FuncBehavior<T>
        where T : Character
    {
        public ActionBehavior(Func<T, float, BehaviorStatus> action)
            : base(action)
        {

        }

    }
    #endregion Action

    #region Condition
    public class ConditionBehavior<T> : FuncBehavior<T>
        where T : Character
    {
        public ConditionBehavior(Func<T, float, BehaviorStatus> condition)
            : base(condition)
        {

        }
    }
    #endregion Condition

    #region Invertor
    public class InvertBehavior<T> : Behavior<T>
        where T : Character
    {
        protected Behavior<T> m_behavior;
        public InvertBehavior(Behavior<T> behavior)
        {
            m_behavior = behavior;
        }

        public override BehaviorStatus Process(T character, float dt)
        {
            var status = m_behavior.Process(character, dt);
            if (status == BehaviorStatus.Success)
                return BehaviorStatus.Failed;
            else if (status == BehaviorStatus.Failed)
                return BehaviorStatus.Success;

            return BehaviorStatus.Running;
        }
    }
    #endregion Invertor

    #region Selector
    public class SelectBehavior<T> : CompositionBehavior<T>
        where T : Character
    {
        public override BehaviorStatus Process(T character, float dt)
        {
            for (var i = 0; i < m_nodes.Count; i++)
            {
                var status = m_nodes[i].Process(character, dt);
                if (status != BehaviorStatus.Failed)
                    return status;
            }

            return BehaviorStatus.Failed;
        }
    }
    #endregion Selector

    #region Sequencer
    public class SequenceBehavior<T> : CompositionBehavior<T>
        where T : Character
    {
        public override BehaviorStatus Process(T character, float dt)
        {
            for (var i = 0; i < m_nodes.Count; i++)
            {
                var status = m_nodes[i].Process(character, dt);
                if (status != BehaviorStatus.Success)
                    return status;
            }

            return BehaviorStatus.Success;
        }
    }
    #endregion Sequencer

    #region Randomizer
    public class RandomBehavior<T> : Behavior<T>
        where T : Character
    {
        protected int m_totalChance;
        protected Behavior<T> m_current;
        protected List<KeyValuePair<int, Behavior<T>>> m_nodes = new List<KeyValuePair<int, Behavior<T>>>();
        public void AddNode(int chance, Behavior<T> node)
        {
            m_totalChance += chance;
            m_nodes.Add(new KeyValuePair<int, Behavior<T>>(m_totalChance, node));
        }

        public override BehaviorStatus Process(T character, float dt)
        {
            if (m_current == null)
            {
                var chance = Random.Range(0, m_totalChance);
                for (var i = 0; i < m_nodes.Count; i++)
                {
                    if (chance < m_nodes[i].Key)
                    {
                        m_current = m_nodes[i].Value;
                        break;
                    }
                }
            }

            var status = m_current.Process(character, dt);
            if (status == BehaviorStatus.Running)
                return BehaviorStatus.Running;

            m_current = null;
            return status;
        }
    }
    #endregion Randomizer
}
