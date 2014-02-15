using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Utils;

namespace BluRay
{
    public partial class BDViewer : UserControl
    {
        INDEX_BDMV indexbdmv;
        public event DataSelectedEventHandler dataSelected;
        public event TabSelectedEventHandler tabSelected;
        public BDViewer()
        {
            InitializeComponent();
        }
        public void Init(INDEX_BDMV indexbdmv)
        {
            #region index
            this.indexbdmv = indexbdmv;
            Stereoscopic.Checked = indexbdmv.app_info.Stereo;
            Stereoscopic.Tag = 0;
            Mode2D.SelectedItem = indexbdmv.app_info.Mode2d;
            Mode2D.Tag = 1;
            Video.SelectedItem = indexbdmv.app_info.VideoFormat;
            Video.Tag = 2;
            FrameRate.SelectedItem = indexbdmv.app_info.FrameRate;
            FrameRate.Tag = 3;
            FirstPlayback.Checked = (indexbdmv.first_play != null);
            FPType.SelectedItem = indexbdmv.first_play.Object_Type;
            FPType.Tag = 0;
            FPPType.SelectedItem = indexbdmv.first_play.Hdmv.Playback_Type;
            playLists.View = View.Details;
            #region fill playlist
            FPIndex.Items.Add(0);
            for (int i = 0; i < indexbdmv.titles.Count; i++)
            {
                ListViewItem lv = new ListViewItem((i + 1).ToString("D3"));
                lv.SubItems.Add(indexbdmv.titles[i].Object_type.ToString());
                lv.SubItems.Add(indexbdmv.titles[i].Permitted);
                lv.Tag = indexbdmv.titles[i];
                if (indexbdmv.titles[i].Object_type == "HDMV")
                {
                    lv.SubItems.Add(indexbdmv.titles[i].Hdmv.Playback_Type);
                    lv.SubItems.Add(indexbdmv.titles[i].Hdmv.id_ref.ToString("D3"));
                }
                else
                    lv.SubItems.Add(indexbdmv.titles[i].Bdj.Playback_Type);
                playLists.Items.Add(lv);
                FPIndex.Items.Add(i + 1);
            }
            //        FPIndex.SelectedItem = indexbdmv.first_play..ListNumber;
            if (playLists.Items.Count > 0) playLists.Items[0].Selected = true;
            #endregion
            // To do : top menu
            #endregion
            #region MovieObj
            Commands.View = View.Details;
            if (indexbdmv.MovieObject != null)
                for (int i = 0; i < indexbdmv.MovieObject.Commands.Count; i++)
                {
                    ListViewItem lv = new ListViewItem(i.ToString("D3"));
                    lv.SubItems.Add(indexbdmv.MovieObject.Commands[i].Resume_intention);
                    lv.SubItems.Add(indexbdmv.MovieObject.Commands[i].Menu_call);
                    lv.SubItems.Add(indexbdmv.MovieObject.Commands[i].Title_search);
                    lv.SubItems.Add(indexbdmv.MovieObject.Commands[i].Commands.Count.ToString());
                    Commands.Items.Add(lv);
                    lv.Tag = indexbdmv.MovieObject.Commands[i];
                }
            if (Commands.Items.Count > 0)
                Commands.Items[0].Selected = true;

            #endregion
            #region Play list
            if (indexbdmv.MplsList != null)
                for (int i = 0; i < indexbdmv.MplsList.Count; i++)
                {
                    list.Items.Add(indexbdmv.MplsList[i]);
                }
            if (list.Items.Count > 0)
                list.SelectedIndex = 0;
            #endregion
            #region Clips
            if (indexbdmv.Clips != null)
                for (int i = 0; i < indexbdmv.Clips.Count; i++)
                {
                    clipsList.Items.Add(indexbdmv.Clips[i]);
                }
            if (clipsList.Items.Count > 0)
                clipsList.SelectedIndex = 0;
            #endregion
        }
        private void Commands_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabSelected != null)
                tabSelected(this, new TabSelectedArgs(Path.GetFileName(indexbdmv.MovieObject.fileName)));
            MOBJ_OBJECT c = (MOBJ_OBJECT)Commands.SelectedItems[0].Tag;
            if (dataSelected != null)
                dataSelected(sender, new DataSelectedEventArgs(c.position, c.length));
 
            Instructions.Items.Clear();
            Instructions.View = View.Details;
            for (int i = 0; i < c.Commands.Count; i++)
            {
                ListViewItem lv = new ListViewItem((i).ToString("D4"));
                lv.SubItems.Add(c.Commands[i].Opcode.ToString("x8"));
                lv.SubItems.Add(c.Commands[i].Destination.ToString("x8"));
                lv.SubItems.Add(c.Commands[i].Source.ToString("x8"));
                lv.SubItems.Add(c.Commands[i].LineCode);
                lv.Tag = c.Commands[i];
                Instructions.Items.Add(lv);
                mobj_print mp = new mobj_print();
              string ss=  mp.mobj_sprint_cmd(c.Commands[i]);
            }
        }
        private void Instructions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabSelected != null)
                tabSelected(this, new TabSelectedArgs(Path.GetFileName(indexbdmv.MovieObject.fileName)));
            MOBJ_CMD c = (MOBJ_CMD)Instructions.SelectedItems[0].Tag;
            if (dataSelected != null)
                dataSelected(sender, new DataSelectedEventArgs(c.position, c.length));
        }

        private void list_SelectedIndexChanged(object sender, EventArgs e)
        {
            MPLS mp = (MPLS)list.SelectedItem;
            if (tabSelected != null)
                tabSelected(this, new TabSelectedArgs(Path.GetFileName(mp.LongName)));
            PlayMode.SelectedItem = mp.App_info.Playback_type;
            uk7.Text = mp.App_info.Playback_count.ToString("x4");
            UserMask1.Text = mp.App_info.Uo_mask.UO1.ToString("X8");
            UserMask_2.Text = mp.App_info.Uo_mask.UO2.ToString("X8");
            r_acces.Checked = mp.App_info.Random_access_flag;
            a_mix.Checked = mp.App_info.Audio_mix_flag;
            b_p_mixer.Checked = mp.App_info.Lossless_bypass_flag;
            #region Fill uo mask flags
            uo_flags.Items.Clear();
            uo_flags.Items.Add("menu call", mp.App_info.Uo_mask.menu_call);
            uo_flags.Items.Add("title search", mp.App_info.Uo_mask.title_search);
            uo_flags.Items.Add("chapter search", mp.App_info.Uo_mask.chapter_search);
            uo_flags.Items.Add("time search", mp.App_info.Uo_mask.time_search);
            uo_flags.Items.Add("skip to next point", mp.App_info.Uo_mask.skip_to_next_point);
            uo_flags.Items.Add("skip to prev point", mp.App_info.Uo_mask.skip_to_prev_point);
            uo_flags.Items.Add("play firstplay", mp.App_info.Uo_mask.play_firstplay);
            uo_flags.Items.Add("stop", mp.App_info.Uo_mask.stop);
            uo_flags.Items.Add("pause on", mp.App_info.Uo_mask.pause_on);
            uo_flags.Items.Add("pause off", mp.App_info.Uo_mask.pause_off);
            uo_flags.Items.Add("still", mp.App_info.Uo_mask.still);
            uo_flags.Items.Add("forward", mp.App_info.Uo_mask.forward);
            uo_flags.Items.Add("backward", mp.App_info.Uo_mask.backward);
            uo_flags.Items.Add("resume", mp.App_info.Uo_mask.resume);
            uo_flags.Items.Add("move up", mp.App_info.Uo_mask.move_up);
            uo_flags.Items.Add("move_down", mp.App_info.Uo_mask.move_down);
            uo_flags.Items.Add("move_left", mp.App_info.Uo_mask.move_left);
            uo_flags.Items.Add("move_right", mp.App_info.Uo_mask.move_right);
            uo_flags.Items.Add("select", mp.App_info.Uo_mask.select);
            uo_flags.Items.Add("activate", mp.App_info.Uo_mask.activate);
            uo_flags.Items.Add("select and activate", mp.App_info.Uo_mask.select_and_activate);
            uo_flags.Items.Add("primary audio change", mp.App_info.Uo_mask.primary_audio_change);
            uo_flags.Items.Add("angle change", mp.App_info.Uo_mask.angle_change);
            uo_flags.Items.Add("popup on", mp.App_info.Uo_mask.popup_on);
            uo_flags.Items.Add("popup off", mp.App_info.Uo_mask.popup_off);
            uo_flags.Items.Add("pg enable disable", mp.App_info.Uo_mask.pg_enable_disable);
            uo_flags.Items.Add("pg change", mp.App_info.Uo_mask.pg_change);
            uo_flags.Items.Add("secondary video enable disable", mp.App_info.Uo_mask.secondary_video_enable_disable);
            uo_flags.Items.Add("secondary video change", mp.App_info.Uo_mask.secondary_video_change);
            uo_flags.Items.Add("secondary audio enable disable", mp.App_info.Uo_mask.secondary_audio_enable_disable);
            uo_flags.Items.Add("secondary audio change", mp.App_info.Uo_mask.secondary_audio_change);
            uo_flags.Items.Add("pip pg change", mp.App_info.Uo_mask.pip_pg_change);
            #endregion
            #region Playlist
            playlist.Items.Clear();
            for (int i = 0; i < mp.Play_item.Count; i++)
            {
                ListViewItem lv = new ListViewItem((i).ToString("D3"));
                lv.Font = new System.Drawing.Font("Microsoft Sans Serif", 7);
                lv.SubItems.Add(mp.Play_item[i].Clip_id);
                lv.SubItems.Add(mp.Play_item[i].Connection_condition.ToString());
                lv.SubItems.Add(mp.Play_item[i].Stc_id.ToString());
                lv.SubItems.Add(FormatTime(mp.Play_item[i].In_time));
                lv.SubItems.Add(FormatTime(mp.Play_item[i].Out_time));
                float diff = mp.Play_item[i].Out_time - mp.Play_item[i].In_time;
                lv.SubItems.Add(FormatTime(diff));
                lv.SubItems.Add(mp.Play_item[i].Uo_mask.UO1.ToString("x8"));
                lv.SubItems.Add(mp.Play_item[i].Uo_mask.UO1.ToString("x8"));
                lv.Tag = (mp.Play_item[i]);
                playlist.Items.Add(lv);
            }
            if (playlist.Items.Count > 0)
                playlist.Items[0].Selected = true;
            #endregion
            #region Play list mark
            playListMark.Items.Clear();
            for (int i = 0; i < mp.Play_mark.Count; i++)
            {
                ListViewItem lv = new ListViewItem((i).ToString("D3"));
                lv.SubItems.Add(mp.Play_mark[i].Mark_Type);
                lv.SubItems.Add(mp.Play_mark[i].Play_item_ref.ToString("D3"));
                lv.SubItems.Add(FormatTime(mp.Play_mark[i].Time));
                lv.SubItems.Add(mp.Play_mark[i].Entry_es_pid.ToString("x4"));
                lv.SubItems.Add(mp.Play_mark[i].Duration.ToString());
                playListMark.Items.Add(lv);
            }
            #endregion
        }
        private string FormatTime(float time)
        {
            string s = "";
            double t = time ;/// 45000;
            int sec = (int)Math.Floor(t);
            int dec = (int)((t - sec) * 1000);
            int min = sec / 60;
            sec = sec % 60;
            int hour = min / 60;
            min = min % 60;
            s = hour.ToString("D2") + ":" + min.ToString("D2") + ":" + sec.ToString("D2") + "." + dec.ToString("D3");
            return s;
        }
        private void playLists_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabSelected != null)
                tabSelected(this, new TabSelectedArgs(Path.GetFileName(indexbdmv.fileName)));
            if (playLists.SelectedItems.Count == 0)
                return;
            INDX_TITLE ind = (INDX_TITLE)playLists.SelectedItems[0].Tag;
            if (dataSelected != null)
                dataSelected(sender, new DataSelectedEventArgs(ind.position, ind.length));
        }
        private void playlist_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (playlist.SelectedItems.Count == 0)
                return;
            MPLS_PI cd = (MPLS_PI)playlist.SelectedItems[0].Tag;
   //         if (tabSelected != null)                tabSelected(this, new TabSelectedArgs(Path.GetFileName(cd.)));

            if(dataSelected!=null)            
                dataSelected(sender, new DataSelectedEventArgs(cd.position,cd.length));
            stnList.Items.Clear();
            ListViewItem lv = new ListViewItem(cd.STN.Num_video.ToString());
            lv.SubItems[0].Tag = "Vi";
            ListViewItem.ListViewSubItem lvsb = new ListViewItem.ListViewSubItem(lv, cd.STN.Num_audio.ToString());
            lvsb.Tag = "Au";
            lv.SubItems.Add(lvsb);
            lvsb = new ListViewItem.ListViewSubItem(lv, cd.STN.Num_pg.ToString());
            lvsb.Tag = "Pg";
            lv.SubItems.Add(lvsb);
            lvsb = new ListViewItem.ListViewSubItem(lv, cd.STN.Num_ig.ToString());
            lvsb.Tag = "Ig";
            lv.SubItems.Add(lvsb);
            lvsb = new ListViewItem.ListViewSubItem(lv, cd.STN.Num_secondary_video.ToString());
            lvsb.Tag = "Sv";
            lv.SubItems.Add(lvsb);
            lvsb = new ListViewItem.ListViewSubItem(lv, cd.STN.Num_secondary_audio.ToString());
            lvsb.Tag = "Sa";
            lv.SubItems.Add(lvsb);
            lvsb = new ListViewItem.ListViewSubItem(lv, cd.STN.Num_pip_pg.ToString());
            lvsb.Tag = "Pip";
            lv.SubItems.Add(lvsb);
            stnList.Items.Add(lv);

        }
        private void clipsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            CLPI cp = (CLPI)clipsList.SelectedItem;
            if (tabSelected != null)
                tabSelected(this, new TabSelectedArgs(Path.GetFileName(cp.LongName)));
            progInfo.Items.Clear();
            int i = 0;
            foreach (CLPI_PROG pr in cp.Program.Progs)
            {
                foreach (CLPI_PROG_STREAM st in pr.Streams)
                {
                    ListViewItem lv = new ListViewItem(i.ToString("D3"));
                    lv.SubItems.Add(st.PID.ToString());
                    lv.SubItems.Add(st.Coding);
                    lv.SubItems.Add(st.Details);
                    i++;
                    progInfo.Items.Add(lv);
                }
            }
            clipType.SelectedItem = cp.Clip.Application_Type;
            rate.Text = cp.Clip.Ts_recording_rate.ToString();
            Packets.Text = cp.Clip.Num_source_packets.ToString();
            ATCd.Checked = cp.Clip.Is_atc_delta;
            i = 0;
            clipInfo.Items.Clear();
            if (cp.Clip.Atc_delta != null)
            {
                foreach (CLPI_ATC_DELTA atcd in cp.Clip.Atc_delta)
                {
                    ListViewItem lv = new ListViewItem(i.ToString("D3"));
                    lv.SubItems.Add(atcd.Delta.ToString("x8"));
                    lv.SubItems.Add(atcd.File_id);
                    lv.SubItems.Add(atcd.File_code);
                    i++;
                    clipInfo.Items.Add(lv);
                }
            }
            if (clipInfo.Items.Count > 0)
                clipInfo.Items[0].Selected = true;
            cpiInfo.Items.Clear();
            i = 0;
            if (cp.Cpi.Entry != null)
            {
                foreach (CLPI_EP_MAP_ENTRY ep in cp.Cpi.Entry)
                {
                    ListViewItem lv = new ListViewItem(i.ToString("D3"));
                    lv.SubItems.Add(ep.PID.ToString("x4"));
                    lv.SubItems.Add(ep.Ep_stream_type.ToString());
                    lv.SubItems.Add(ep.Coarse.Count.ToString("x4"));
                    lv.SubItems.Add(ep.Fine.Count.ToString("x4"));
                    lv.SubItems.Add(ep.ep_map_stream_start_addr.ToString());
                    lv.Tag = ep;
                    i++;
                    cpiInfo.Items.Add(lv);
                }
            }
            if (cpiInfo.Items.Count > 0)
                cpiInfo.Items[0].Selected = true;
            i = 0;
            sequenceInfo.Items.Clear();
            foreach (CLPI_ATC_SEQ atcSeq in cp.Sequence.Atc_seq)
            {
                ListViewItem lv = new ListViewItem(i.ToString("D3"));
                lv.SubItems.Add(atcSeq.spn_atc_start.ToString("x8"));
                lv.SubItems.Add(atcSeq.num_stc_seq.ToString());
                lv.SubItems.Add(atcSeq.offset_stc_id.ToString());
                lv.Tag = atcSeq;
                sequenceInfo.Items.Add(lv); 
                i++;
            }
            if (sequenceInfo.Items.Count > 0)
                sequenceInfo.Items[0].Selected = true;

        }

        private void cpiInfo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cpiInfo.SelectedItems.Count == 0)
                return;
            CLPI_EP_MAP_ENTRY ep = (CLPI_EP_MAP_ENTRY)cpiInfo.SelectedItems[0].Tag;
            int i = 0;
            coarse.Items.Clear();
            foreach (CLPI_EP_COARSE epCoarse in ep.Coarse)
            {
                ListViewItem lv = new ListViewItem(i.ToString("D3"));
                lv.SubItems.Add(epCoarse.Ref_ep_fine_id.ToString());
                lv.SubItems.Add(epCoarse.Pts_ep.ToString());
                lv.SubItems.Add(epCoarse.Spn_ep.ToString());
                coarse.Items.Add(lv);
                i++;
            }
            i = 0;
            fine.Items.Clear();
            foreach (CLPI_EP_FINE epfine in ep.Fine)
            {
                ListViewItem lv = new ListViewItem(i.ToString("D3"));
                lv.SubItems.Add(epfine.Is_angle_change_point.ToString());
                lv.SubItems.Add(epfine.I_end_position_offset.ToString());
                lv.SubItems.Add(epfine.Pts_ep.ToString());
                lv.SubItems.Add(epfine.Spn_ep.ToString());
                fine.Items.Add(lv);
                i++;
            }
        }

        private void sequenceInfo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sequenceInfo.SelectedItems.Count == 0)
                return;
            CLPI_ATC_SEQ atcSeq = (CLPI_ATC_SEQ)sequenceInfo.SelectedItems[0].Tag;
            int i = 0;
            pcrInfo.Items.Clear();
            foreach (CLPI_STC_SEQ stcSeq in atcSeq.Stc_seq)
            {
                ListViewItem lv = new ListViewItem(i.ToString("D3"));
                lv.SubItems.Add(stcSeq.Pcr_pid.ToString());
                lv.SubItems.Add(FormatTime(stcSeq.Presentation_start_time));
                lv.SubItems.Add(FormatTime(stcSeq.Presentation_end_time));
                i++;
                pcrInfo.Items.Add(lv);
            }
        }
        private void stnList_MouseDown(object sender, MouseEventArgs e)
        {
            if (playlist.SelectedItems.Count == 0)
                return;
            MPLS_PI cd = (MPLS_PI)playlist.SelectedItems[0].Tag;
            ListViewHitTestInfo lvt = stnList.HitTest((e.Location));
            ListViewItem.ListViewSubItem sub = lvt.SubItem;
            List<MPLS_STREAM> streams = null;

            string m = (string)sub.Tag; ;
            switch (m)
            {
                case "Vi":
                    streams = cd.STN.Video;
                    break;
                case "Au":
                    streams = cd.STN.Audio;
                    break;
                case "Pg":
                    streams = cd.STN.Pg;
                    break;
                case "Ig":
                    streams = cd.STN.Ig;
                    break;
                case "Sv":
                    streams = cd.STN.Secondary_video;
                    break;
                case "Sa":
                    streams = cd.STN.Secondary_audio;
                    break;
            }
            stnDet.Items.Clear();
            if (streams != null)
            {
                int i = 1;
                foreach (MPLS_STREAM st in streams)
                {
                    ListViewItem lv = new ListViewItem((i).ToString("D3"));
                    lv.SubItems.Add(st.Stream_Type);
                    lv.SubItems.Add(st.Pid.ToString("x4"));
                    lv.SubItems.Add(st.Subclip_id.ToString("x2"));
                    lv.SubItems.Add(st.Subpath_id.ToString("x2"));
                    lv.SubItems.Add("");
                    lv.SubItems.Add(st.Format + "," + st.FrameRate + "," + st.Lang);
                    stnDet.Items.Add(lv);
                }

            }

        }

        private void savePlayListBtn_Click(object sender, EventArgs e)
        {

        }

        private void Mode2D_MouseDown(object sender, MouseEventArgs e)
        {
            Control c = (Control)sender;
            if (tabSelected != null)
                tabSelected(this, new TabSelectedArgs(Path.GetFileName(indexbdmv.fileName)));

            if (c.Tag != null)
            {
                int i = (int)c.Tag;
                System.Reflection.PropertyInfo[] x = typeof(INDX_APP_INFO).GetProperties();
                BinPosition pos = new BinPosition(indexbdmv.app_info.FieldPosition[i], x[i].Name, x[i].PropertyType, null);
                if (dataSelected != null)
                    dataSelected(sender, new DataSelectedEventArgs(indexbdmv.app_info.position, indexbdmv.app_info.length, pos));
            }
        }

    }
}
