using UnityEngine;using System.Collections.Generic;
using Phenix.Unity.Collection;

namespace Phenix.Unity.Effect
{
    public class ParticleMgr
    {
        class ParticleInst
        {
            public GameObject inst;             // prefab实例
            public ParticleSystem[] emitters;   // inst及子对象的所有ParticleSystem组件
            public bool finished = false;       // 所有emitter运行结束
        }

        class ParticleData
        {
            public int particleCode;
            public GameObject prefab;           // 粒子对象预设体（可能包含若干子对象）
            public Pool<ParticleInst> pool;     // ParticleInst池

            public List<ParticleInst> runList = new List<ParticleInst>();  // 运行中的ParticleInst
            public List<ParticleInst> finishList = new List<ParticleInst>(); // 运行结束的ParticleInst

            public ParticleData(int particleCode, GameObject prefab, int poolCapacity)
            {
                this.particleCode = particleCode;
                this.prefab = prefab;
                pool = new Pool<ParticleInst>(poolCapacity);
            }
        }

        Dictionary<int/*粒子类型*/, ParticleData> _particles = new Dictionary<int, ParticleData>();        

        public void Add(int particleCode, GameObject prefab, int poolCapacity)
        {            
            _particles.Add(particleCode, new ParticleData(particleCode, prefab, poolCapacity));
        }

        public GameObject Play(int particleType, Vector3 pos, Vector3 dir)
        {
            return PlayImpl(particleType, null, pos, dir);
        }

        public GameObject Play(int particleType, Transform parent, Vector3 pos, Vector3 dir)
        {
            return PlayImpl(particleType, parent, pos, dir);
        }

        GameObject PlayImpl(int particleType, Transform parent, Vector3 pos, Vector3 dir)
        {
            if (_particles.ContainsKey(particleType) == false)
            {
                return null;
            }

            ParticleData data = _particles[particleType];
            ParticleInst newParticleInst = data.pool.Get();
            if (newParticleInst.inst == null)
            {                
                newParticleInst.inst = GameObject.Instantiate(data.prefab) as GameObject;
                newParticleInst.emitters = newParticleInst.inst.GetComponentsInChildren<ParticleSystem>();
            }

            newParticleInst.inst.transform.SetParent(parent);
            newParticleInst.inst.transform.position = pos;               
            newParticleInst.inst.transform.forward = dir;
            newParticleInst.inst.SetActive(true);
            foreach (var item in newParticleInst.emitters)
            {
                item.Play();
            }

            data.runList.Add(newParticleInst);

            return newParticleInst.inst;
        }

        public void Stop(GameObject particlePrefabInst)
        {
            // 遍历所有
            foreach (var item in _particles)
            {
                // 遍历running
                foreach (var run in item.Value.runList)
                {
                    if (run.inst == particlePrefabInst)
                    {
                        foreach (var particleSystem in run.emitters)
                        {
                            // 停止发射粒子
                            particleSystem.Stop();
                        }

                        return;
                    }
                }
            }
        }

        public void OnUpdate()
        {            
            // 遍历管理单元中的所有种类粒子
            foreach (var item in _particles)
            {
                // 遍历运行中的ParticleInst，check是否完成
                foreach (var run in item.Value.runList)
                {
                    run.finished = true;
                    foreach (ParticleSystem particle in run.emitters)
                    {
                        if (particle.IsAlive())
                        {
                            run.finished = false;
                            break;
                        }
                    }

                    if (run.finished)
                    {
                        item.Value.finishList.Add(run);
                    }
                }

                foreach (var finish in item.Value.finishList)
                {
                    item.Value.pool.Collect(finish); // 回收已完成的ParticleInst
                    item.Value.runList.Remove(finish);
                }

                item.Value.finishList.Clear();
            }            
        }
    }
    
}