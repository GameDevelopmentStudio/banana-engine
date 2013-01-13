using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace bEngine
{
    public class bGameState
    {
        public bGame game;

        protected Dictionary<string, List<bEntity>> entities;
        protected Dictionary<bEntity, string> categories;
        protected List<bEntity> deathRow;
        protected List<Pair<bEntity, String>> birthRow;

        public bGameState()
        {
            entities = new Dictionary<string, List<bEntity>>();
            categories = new Dictionary<bEntity, string>();
            deathRow = new List<bEntity>();
            birthRow = new List<Pair<bEntity, String>>();
        }

        virtual public void init()
        {
            Console.WriteLine("GameLevel init!");
        }

        virtual public void end()
        {
            Console.WriteLine("GameLevel end!");
        }

        virtual public void update(GameTime dt)
        {
        }

        virtual public void render(GameTime dt, SpriteBatch sb, Matrix matrix)
        {
        }

        virtual public bool add(bEntity ge, String category)
        {
            birthRow.Add(new Pair<bEntity, String>(ge, category));
            return true;
        }

        virtual protected bool _add(bEntity e, String category)
        {
            // Store container list
            categories[e] = category;

            e.world = this;
            e.game = this.game;
            e.init();
            return true;
        }

        virtual public void remove(bEntity e)
        {
            deathRow.Add(e);
        }

        virtual public void actuallyRemove(bEntity e)
        {
            String c = categories[e];
            if (c != null)
                entities[c].Remove(e);
        }

        virtual public bool collides(bEntity e, string[] categories, Func<bEntity, bEntity, bool> condition = null)
        {
            foreach (string category in categories)
                if (entities[category] != null)
                    foreach (bEntity ge in entities[category])
                        if (ge != e && ge.collidable && e.collides(ge) && (condition == null || condition(e, ge)))
                            return true;
            return false;
        }

        virtual public bEntity instanceCollision(bEntity e, string category, string attr = null, Func<bEntity, bEntity, bool> condition = null)
        {
            if (entities[category] != null)
                foreach (bEntity ge in entities[category])
                    if (ge != e && ge.collidable && e.collides(ge) && (condition == null || condition(e, ge)))
                        if (attr == null || ge.hasAttribute(attr))
                            return ge;
            return null;
        }

        virtual public List<bEntity> instancesCollision(bEntity e, string category, string attr = null)
        {
            List<bEntity> result = new List<bEntity>();

            if (entities[category] != null)
                foreach (bEntity ge in entities[category])
                    if (ge != e && ge.collidable && e.collides(ge))
                        if (attr == null || ge.hasAttribute(attr))
                            result.Add(ge);

            return result;
        }

        virtual public bEntity find(int id)
        {
            foreach (String key in entities.Keys)
            {
                foreach (bEntity e in entities[key])
                {
                    if (e.id == id)
                        return e;
                }
            }

            return null;
        }

        virtual public int instanceNumber(Type target)
        {
            int count = 0;
            foreach (String key in entities.Keys)
            {
                foreach (bEntity e in entities[key])
                    if (target.IsInstanceOfType(e))
                        count += 1;
            }
            return count;
        }

        virtual public bool isInstanceInView(bEntity e)
        {
            // TODO: Handle invisible and not managed by world entities
            return true;
        }
    }
}
